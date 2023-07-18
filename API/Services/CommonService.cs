using MagazinchikAPI.DTO;
using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.Services
{
    public class CommonService
    {

        private readonly ApplicationDbContext _context;

        public CommonService(ApplicationDbContext context)
        {
            _context = context;
        }
        
         public async Task<long> UserIsOk(HttpContext context)
        {
            long jwtId = Convert.ToInt64(context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new APIException("Broken acces token", 401));
            _ = await _context.Users.FindAsync(jwtId) ?? throw new APIException("Undefined user", 401);

            return jwtId;
        }

        //returns null if not ok
        public async Task<long?> UserIsOkNullable(HttpContext context)
        {
            //if there's no token at all, return null
            var HeaderJwtKey= context.Request.Headers.Authorization;
            if(HeaderJwtKey.IsNullOrEmpty()) return null;

            //if there's some token, but it is invalid => 401
            var jwtKey = context.User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new APIException("Token is not null, but not valid. Try refresh", 401);

            var jwtId = Convert.ToInt64(jwtKey);
            var result = await _context.Users.FindAsync(jwtId)
            ?? throw new APIException("Undefined user", 401);

            return result.Id;
        }

         private static bool IsFavourite(Product product, long? userId)
        {
            if (product.Favourites == null || userId == null) return false;

            if (product.Favourites.Where(x => x.UserId == userId).IsNullOrEmpty()) return false;

            return true;
        }

        private static bool IsInCart(Product product, long? userId)
        {
            if (product.CartProducts == null || userId == null) return false;

            if (product.CartProducts.Where(x => x.UserId == userId).IsNullOrEmpty()) return false;

            return true;
        }

        public static void SetFlags(List<ProductDtoBaseInfo> inputDtos, IEnumerable<Product> inputProducts, long? userId)
        {
            inputDtos.ForEach(x => x.IsFavourite = IsFavourite(inputProducts.First(y => y.Id == x.Id), userId));
            inputDtos.ForEach(x => x.IsInCart = IsInCart(inputProducts.First(y => y.Id == x.Id), userId));
        }
        public static void SetFlags(ProductDtoDetailed inputDto, Product inputProduct, long? userId)
        {
            inputDto.IsFavourite = IsFavourite(inputProduct, userId);
            inputDto.IsInCart = IsInCart(inputProduct, userId);
        }
    }
}