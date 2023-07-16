using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.CartProduct;
using MagazinchikAPI.Infrastructure;

namespace MagazinchikAPI.Services
{
    public class CartService : ICartService
    {
        private const int LIMIT_SIZE = 50;
        private readonly ApplicationDbContext _context;
        private readonly CommonService _commonService;
        private readonly IMapper _mapper;
        public CartService(ApplicationDbContext context, CommonService commonService, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _commonService = commonService;
        }

        public async Task AddToCart(long productId, HttpContext context)
        {
            var jwtId = await _commonService.UserIsOk(context);


            var productToAdd = _context.CartProducts.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId);

            if (productToAdd == null)
            {
                //Check if product exists
                _ = await _context.Products.FindAsync(productId)
            ?? throw new APIException("Product does not exist", 404);

                //Create new CartProduct
                productToAdd = new()
                {
                    ProductId = productId,
                    UserId = jwtId,
                    ProductCount = 1
                };

                await _context.CartProducts.AddAsync(productToAdd);
            }

            else productToAdd.ProductCount++;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCart(long productId, HttpContext context)
        {
            var jwtId = await _commonService.UserIsOk(context);

            var productToRemove = _context.CartProducts.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId)
            ?? throw new APIException("Nothing to remove", 404);

            _context.CartProducts.Remove(productToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task DecreaseFromCart(long productId, HttpContext context)
        {
            var jwtId = await _commonService.UserIsOk(context);


            var productToDecrease = _context.CartProducts.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId)
            ?? throw new APIException("Nothing to decrease", 404);

            if (productToDecrease.ProductCount != 0) productToDecrease.ProductCount--;
            else _context.CartProducts.Remove(productToDecrease);


            await _context.SaveChangesAsync();
        }

        public async Task<Page<CartProductDtoBaseInfo>> GetAllForUser(HttpContext context, int limit, int offset)
        {
            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);

            var jwtId = await _commonService.UserIsOk(context);

            var elementsCount = _context.CartProducts.Where(x => x.UserId == jwtId).Count();
            if(elementsCount == 0) throw new APIException("Nothing found", 404);

            var pages = (int)Math.Ceiling((float)elementsCount / (float)limit);
            if (offset > pages - 1 || offset < 0) throw new APIException($"Invalid offset: {offset}", 400);

            var productsFromCart = await _context.CartProducts
            .Include(x => x.Product)
            .ThenInclude(y => y!.Photos)
            .Where(x => x.UserId == jwtId)
            .ProjectTo<CartProductDtoBaseInfo>(_mapper.ConfigurationProvider).ToListAsync();

            return new Page<CartProductDtoBaseInfo>() { CurrentOffset = offset, CurrentPage = productsFromCart, Pages = pages };
        }
    
    }
}