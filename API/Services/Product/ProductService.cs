using System.Text.Json;
using FluentValidation;
using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Review;
using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.Services
{
    public class ProductService : IProductService
    {
        private const int LIMIT_SIZE = 50;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly CommonService _commonService;

        public ProductService(ApplicationDbContext context, IMapper mapper, CommonService commonService)
        {
            _commonService = commonService;
            _context = context;
            _mapper = mapper;
        }
        public async Task Create(ProductDtoCreate input)
        {
            var productToSave = _mapper.Map<Product>(input);
            (productToSave.UpdatedAt, productToSave.CreatedAt) = (DateTime.UtcNow, DateTime.UtcNow);

            var cathegory = _context.Cathegories.Find(productToSave.CathegoryId)
            ?? throw new APIException("Invalid cathegory", 404);
            if (cathegory.IsParent) throw new APIException("Invalid cathegory", 400);

            await _context.Products.AddAsync(productToSave);
            await _context.SaveChangesAsync();
        }
        public async Task<List<ProductDtoBaseInfo>> GetAll()
        {
            var result = new List<ProductDtoBaseInfo>();
            var rawResult = await _context.Products.Include(x => x.Cathegory).Include(x => x.Photos).ToListAsync();
            foreach (var element in rawResult)
            {
                FindAllParents(element.Cathegory);
                result.Add(_mapper.Map<ProductDtoBaseInfo>(element));
            }
            return result;
        }
        public async Task<ProductDtoBaseInfo> GetBaseInfo(long productId)
        {
            var result = await _context.Products
            .Include(x => x.Cathegory)
            .Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == productId)
            ?? throw new APIException("No such product", 404);
            FindAllParents(result.Cathegory);

            return _mapper.Map<ProductDtoBaseInfo>(result);
        }
        public Page<ProductDtoBaseInfo> GetPopular(int limit, int offset)
        {
            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);

            var pages = (int)Math.Ceiling((float)_context.Products.Count() / (float)limit);
            if (offset > pages - 1 || offset < 0) throw new APIException($"Invalid offset: {offset}", 400);

            var pageData = _mapper.Map<List<ProductDtoBaseInfo>>(
                _context.Products
                .OrderByDescending(x => x.Purchases)
                .Skip(offset * limit)
                .Take(limit));

            return new Page<ProductDtoBaseInfo>() { CurrentOffset = offset, CurrentPage = pageData, Pages = pages };


        }
        public async Task<List<ProductDtoBaseInfo>> GetRandomPersonal(HttpContext httpContext, int limit)
        {
            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);

            var jwtId = await _commonService.UserIsOk(httpContext);

            var idsFromCookies = LoadExceptProductsFromCookies(httpContext);

            var favouriteCathegoriesIds = _context.Favourites.Include(x => x.Product).Where(x => x.UserId == jwtId).Select(x => x.Product!.CathegoryId).ToList();

            var result = _context.Products.Where(x => favouriteCathegoriesIds.Contains(x.CathegoryId))
            .OrderBy(x => EF.Functions.Random()).ToList()
            .ExceptBy(idsFromCookies, x => x.Id)
            .Take(limit);

            SaveExceptProductsToCookies(httpContext, result.Select(x => x.Id));

            return _mapper.Map<List<ProductDtoBaseInfo>>(result);
        }
        public List<ProductDtoBaseInfo> GetRandomByCathegory(long cathegoryId, HttpContext httpContext, int limit)
        {
            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);


            var idsFromCookies = LoadExceptProductsFromCookies(httpContext);


            var result = _context.Products.Where(x => x.CathegoryId == cathegoryId)
            .OrderBy(x => EF.Functions.Random()).Take(limit).ToList().ExceptBy(idsFromCookies, x => x.Id);

            SaveExceptProductsToCookies(httpContext, result.Select(x => x.Id));

            return _mapper.Map<List<ProductDtoBaseInfo>>(result);
        }

        public async Task AddToFavourite(long productId, HttpContext context)
        {

            var jwtId = await _commonService.UserIsOk(context);


            if (_context.Favourites.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId) != null)
                throw new APIException("Already added to favourites", 400);

            _ = await _context.Products.FindAsync(productId)
            ?? throw new APIException("Product does not exist", 404);


            var FavouriteToSave = new Favourite { ProductId = productId, UserId = jwtId };

            await _context.Favourites.AddAsync(FavouriteToSave);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromFavourite(long productId, HttpContext context)
        {
            var jwtId = await _commonService.UserIsOk(context);

            var favouriteToRemove = _context.Favourites.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId)
            ?? throw new APIException("Nothing to remove", 404);

            _context.Favourites.Remove(favouriteToRemove);
            await _context.SaveChangesAsync();
        }

        private void FindAllParents(Cathegory? cathegory)
        {
            if (cathegory == null) return;
            cathegory.Parent = _context.Cathegories.Find(cathegory.ParentId);
            if (cathegory.Parent != null) FindAllParents(cathegory.Parent);
        }

        private static void SaveExceptProductsToCookies(HttpContext context, IEnumerable<long> ids)
        {
            //NB а что если куки наберется больше 4 кб?
            var idsFromCookies = new List<long>();

            if (context.Request.Cookies.ContainsKey("except_products"))
            {
                idsFromCookies = JsonSerializer.Deserialize<List<long>>(context.Request.Cookies["except_products"] ?? throw new Exception("can't deserialize cookie"))
                ?? throw new Exception("can't deserialize cookie");
            }

            idsFromCookies.AddRange(ids);
            idsFromCookies = idsFromCookies.Distinct().ToList();

            context.Response.Cookies.Delete("except_products");
            context.Response.Cookies.Append("except_products", JsonSerializer.Serialize(idsFromCookies),
            new CookieOptions() { Secure = true, HttpOnly = true, MaxAge = new TimeSpan(365, 0, 0, 0) });
        }

        private static IEnumerable<long> LoadExceptProductsFromCookies(HttpContext context)
        {
            if (!context.Request.Cookies.ContainsKey("except_products"))
            {
                return Enumerable.Empty<long>();
            }

            var idsFromCookies = JsonSerializer.Deserialize<List<long>>(context.Request.Cookies["except_products"] ?? throw new Exception("can't deserialize cookie"))
            ?? throw new Exception("can't deserialize cookie");

            return idsFromCookies;
        }
    }


}