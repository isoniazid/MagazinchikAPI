using MagazinchikAPI.DTO;
using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;
using Microsoft.Extensions.Caching.Distributed;

namespace MagazinchikAPI.Services
{
    public class CommonService
    {

        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly string _recordPrefix = "MagazinhikAPI_UserCheck__";

        public CommonService(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        private async Task<User?> TryGetUserFromCache(long userId)
        {
            string recordKey = $"{_recordPrefix}{DateTime.Now:dd_MM_yyyy_hh_mm}__{userId}";

            var result = await _cache.GetRecordAsync<User>(recordKey);

            //NB need to add logger...
                if(result is not null)
                {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Loaded usr from cache");
                Console.ResetColor();
                }


            return result;
        }

        private async Task SaveUserToCache(User user)
        {
            string recordKey = $"{_recordPrefix}{DateTime.Now.ToString("dd_MM_yyyy_hh_mm")}__{user.Id}";
            await _cache.SetRecordAsync(recordKey, user);
        }

        public async Task<long> UserIsOk(HttpContext context)
        {
            long jwtId = Convert.ToInt64(context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new APIException("Broken acces token", 401));

            var cachedUser = await TryGetUserFromCache(jwtId);

            if (cachedUser is null)
            {
                var userToCache = await _context.Users.FindAsync(jwtId) ?? throw new APIException("Undefined user", 401);
                await SaveUserToCache(userToCache);
            }

            return jwtId;
        }

        //returns null if not ok
        public async Task<long?> UserIsOkNullable(HttpContext context)
        {
            //if there's no token at all, return null
            var HeaderJwtKey = context.Request.Headers.Authorization;
            if (HeaderJwtKey.IsNullOrEmpty()) return null;

            //if there's some token, but it is invalid => 401
            var jwtKey = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new APIException("Token is not null, but not valid. Try refresh", 401);

            var jwtId = Convert.ToInt64(jwtKey);

            var result = await TryGetUserFromCache(jwtId) ?? await _context.Users.FindAsync(jwtId)
            ?? throw new APIException("Undefined user", 401);


            return result.Id;
        }

        public static bool IsFavourite(Product product, long? userId)
        {
            if (product.Favourites == null || userId == null) return false;

            if (product.Favourites.Where(x => x.UserId == userId).IsNullOrEmpty()) return false;

            return true;
        }

        public static bool IsInCart(Product product, long? userId)
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