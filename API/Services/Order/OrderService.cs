using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly CommonService _commonService;
        public OrderService(ApplicationDbContext context, CommonService commonService)
        {
            _commonService = commonService;
            _context = context;
        }

        public async Task CreateOrder(HttpContext context, long addressId)
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