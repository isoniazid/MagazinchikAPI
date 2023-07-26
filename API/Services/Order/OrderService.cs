using MagazinchikAPI.Services.Payment;
using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;
using MagazinchikAPI.DTO.Order;
using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Services
{
    public class OrderService : IOrderService
    {
        private const int LIMIT_SIZE = 50;
        private readonly ApplicationDbContext _context;
        private readonly CommonService _commonService;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        public OrderService(ApplicationDbContext context, CommonService commonService, IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _commonService = commonService;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Page<OrderDtoBaseInfo>> GetAllForUser(HttpContext context, int limit, int offset)
        {
             if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);

            var jwtId = await _commonService.UserIsOk(context);

            var elementsCount = _context.Orders.Where(x => x.UserId == jwtId).Count();

            var pages = Page.CalculatePagesAmount(elementsCount, limit);
            if(pages <= 0) return new Page<OrderDtoBaseInfo>();
            if (!Page.OffsetIsOk(offset, pages)) throw new APIException($"Invalid offset: {offset}", 400);

            var orders = await _context.Orders
            .Include(x => x.Address)
            .Include(x => x.OrderProducts!)
            .ThenInclude(y => y.Product)
            .ThenInclude(z => z!.Photos)
            .Where(x => x.UserId == jwtId)
            .OrderByDescending(x => x.Id)
            .Skip(offset * limit)
            .Take(limit)
            .ProjectTo<OrderDtoBaseInfo>(_mapper.ConfigurationProvider)
            .ToListAsync();

             return new Page<OrderDtoBaseInfo>()
            { CurrentOffset = offset, CurrentPage = orders, Pages = pages, ElementsCount = elementsCount };
        }

        public async Task<OrderDtoBaseInfo> GetById(HttpContext context, long orderId)
        {
            var jwtId = await _commonService.UserIsOk(context);

            var result = await _context.Orders
            .Include(x => x.Address)
            .Include(x => x.OrderProducts!)
            .ThenInclude(y => y.Product)
            .ThenInclude(z => z!.Photos)
            .Where(x => x.UserId == jwtId)
            .ProjectTo<OrderDtoBaseInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == orderId)
            ?? throw new APIException("order for authorized user not found", 404);

            return result;
        }

        public async Task CheckPaymentsForOrders()//For Quartz
        {
            var ordersToCheck = await _context.Orders.Where(x => x.OrderStatus == OrderStatus.AWAITING && x.PaymentId != null).ToListAsync();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Checking payment states for {ordersToCheck.Count} orders...");
            Console.ResetColor();

            for (int i = 0; i < ordersToCheck.Count; ++i)
            {
                await HandlePaymentState(ordersToCheck[i]);
            }

            await _context.SaveChangesAsync();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Checked {ordersToCheck.Count}");
            Console.ResetColor();
        }

        public async Task CheckPaymentForOrders(HttpContext context) //For manual checking
        {
            var jwtId = await _commonService.UserIsOk(context);


            var ordersToCheck = await _context.Orders.Where(x => x.OrderStatus == OrderStatus.AWAITING
             && x.PaymentId != null
             && x.UserId == jwtId).ToListAsync();

            for (int i = 0; i < ordersToCheck.Count; ++i)
            {
                await HandlePaymentState(ordersToCheck[i]);
            }

            await _context.SaveChangesAsync();

        }

        private async Task HandlePaymentState(Order order)
        {
            var paymentState = await _paymentService.GetPaymentStatus(order.PaymentId ?? throw new Exception("Null PaymentId"));
            switch (paymentState)
            {
                case PaymentState.AWAIT_PAYMENT:
                    //do nothing and continue asking
                    break;

                case PaymentState.READY_TO_CAPTURE:
                    await _paymentService.CapturePayment(order.PaymentId);
                    break;

                case PaymentState.PAID:
                    order.OrderStatus = OrderStatus.PROCESSING;
                    order.UpdatedAt = DateTime.UtcNow;
                    break;

                case PaymentState.CANCELLED:
                    order.OrderStatus = OrderStatus.CANCELLED;
                    order.UpdatedAt = DateTime.UtcNow;
                    break;

                case PaymentState.UNKNOWN:
                    throw new Exception($"UNKNOWN PAYMENT STATE! {order.PaymentId}");

                default:
                    throw new Exception($"UNKNOWN PAYMENT STATE! Probably, not implemented? {order.PaymentId}");
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Ended Cycle of requesting payment state");
            Console.ResetColor();
        }

        public async Task<OrderPaymentDto> PayForOrder(long orderId, HttpContext context)
        {
            var jwtId = await _commonService.UserIsOk(context);

            var orderToPay = await _context.Orders
            .Include(x => x.OrderProducts)
            .FirstOrDefaultAsync(x => x.Id == orderId)
            ?? throw new APIException("No such order", 404);

            if (orderToPay.UserId != jwtId)
                throw new APIException("This order does not relate to current user", 401);

            if (orderToPay.OrderStatus != OrderStatus.AWAITING)
                throw new APIException("The order is already paid or cancelled", 400);

            var (url, paymentId) = await _paymentService.InitiatePayment(orderToPay);

            orderToPay.PaymentId = paymentId;

            await _context.SaveChangesAsync();

            return new OrderPaymentDto{Url = url, OrderId = orderToPay.Id};
        }

        public async Task<long> CreateOrder(HttpContext context, long addressId)
        {
            var jwtId = await _commonService.UserIsOk(context);


            var address = await _context.Addresses.FindAsync(addressId)
            ?? throw new APIException("Address does not exist", 404);
            if (address.UserId != jwtId)
                throw new APIException("The address does not relate to user", 401);


            var orderToSave = new Order()
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AddressId = addressId,
                UserId = jwtId
            };

            var orderProducts = await CreateOrderProducts(jwtId, orderToSave);

            orderToSave.OrderProducts = orderProducts;
            orderToSave.Price = orderProducts.Select(x => x.TotalPrice).Sum();

            await _context.Orders.AddAsync(orderToSave);
            await _context.OrderProducts.AddRangeAsync(orderProducts);

            await _context.SaveChangesAsync();

            await ClearCart(jwtId);

            return orderToSave.Id;
        }

        private async Task<List<OrderProduct>> CreateOrderProducts(long userId, Order order)
        {
            var orderProducts = new List<OrderProduct>();

            var userCartProducts = await _context.CartProducts.Where(x => x.UserId == userId).ToListAsync();
            if (userCartProducts.IsNullOrEmpty()) throw new APIException("Nothing in cart", 404);

            foreach (var element in userCartProducts)
            {
                var totalPrice = await CalculateProductTotalPrice(element.ProductId, element.ProductCount);

                orderProducts.Add(new()
                {
                    Order = order,
                    ProductId = element.ProductId,
                    ProductCount = element.ProductCount,
                    TotalPrice = totalPrice
                });
            }

            return orderProducts;
        }

        private async Task ClearCart(long userId)
        {
            var elementsToClear = await _context.CartProducts.Where(x => x.UserId == userId).ToListAsync();
            if (elementsToClear.IsNullOrEmpty()) throw new Exception("Nothing to clear!");

            _context.CartProducts.RemoveRange(elementsToClear);
            await _context.SaveChangesAsync();
        }

        private async Task<decimal> CalculateProductTotalPrice(long? productId, long productCount)
        {
            var product = await _context.Products.FindAsync(productId)
                ?? throw new APIException("no such product", 404);

            return productCount * product.Price;
        }

    }
}