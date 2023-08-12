using System.Text.Json;
using FluentValidation;
using MagazinchikAPI.DTO;
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
        {//NB NO VALIDATORS!!!
            var productToSave = _mapper.Map<Product>(input);
            (productToSave.UpdatedAt, productToSave.CreatedAt) = (DateTime.UtcNow, DateTime.UtcNow);

            var cathegory = _context.Cathegories.Find(productToSave.CathegoryId)
            ?? throw new APIException("Invalid cathegory", 404);
            if (cathegory.IsParent) throw new APIException("Invalid cathegory", 400);

            await _context.Products.AddAsync(productToSave);
            await _context.SaveChangesAsync();
        }
        public async Task<Page<ProductDtoBaseInfo>> GetAll(int limit, int offset, HttpContext context)
        {
            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);

            var elementsCount = _context.Products.Count();

            var pages = Page.CalculatePagesAmount(elementsCount, limit);
            if (pages <= 0) return new Page<ProductDtoBaseInfo>();
            if (!Page.OffsetIsOk(offset, pages)) throw new APIException($"Invalid offset: {offset}", 400);

            var jwtId = await _commonService.UserIsOkNullable(context);

            var result = new List<ProductDtoBaseInfo>();
            var rawResult = await _context.Products
            .Include(x => x.Cathegory)
            .Include(x => x.Photos)
            .Include(x => x.Favourites)
            .Include(x => x.CartProducts)
            .OrderBy(x => x.Id)
            .Skip(offset * limit)
            .Take(limit)
            .ToListAsync();


            foreach (var element in rawResult)
            {
                FindAllParents(element.Cathegory);
                result.Add(_mapper.Map<ProductDtoBaseInfo>(element));
            }

            if (jwtId != null) CommonService.SetFlags(result, rawResult, jwtId);

            return new Page<ProductDtoBaseInfo>()
            { CurrentOffset = offset, CurrentPage = result, Pages = pages, ElementsCount = elementsCount };
        }
        public async Task<ProductDtoDetailed> GetDetailedInfo(long productId, HttpContext httpContext)
        {
            var jwtId = await _commonService.UserIsOkNullable(httpContext);

            var rawResult = await _context.Products
            .Include(x => x.Cathegory)
            .Include(x => x.Photos)
            .Include(x => x.Reviews)
            .Include(x => x.Favourites)
            .Include(x => x.CartProducts)
            .FirstOrDefaultAsync(x => x.Id == productId)
            ?? throw new APIException("No such product", 404);

            FindAllParents(rawResult.Cathegory);



            var result = _mapper.Map<ProductDtoDetailed>(rawResult);

            if(jwtId != null) result.SetFlags(rawResult, (long)jwtId);
            //SaveExceptProductToCookies(httpContext, result.Id);

            return result;

        }
        public async Task<Page<ProductDtoBaseInfo>> GetPopular(int limit, int offset, HttpContext context)
        {

            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);

            var elementsCount = _context.Products.Count();

            var pages = Page.CalculatePagesAmount(elementsCount, limit);
            if (pages <= 0) return new Page<ProductDtoBaseInfo>();
            if (!Page.OffsetIsOk(offset, pages)) throw new APIException($"Invalid offset: {offset}", 400);

            var jwtId = await _commonService.UserIsOkNullable(context);

            var rawData =
                _context.Products
                .Include(x => x.Photos)
                .Include(x => x.Favourites)
                .Include(x => x.CartProducts)
                .OrderByDescending(x => x.Purchases)
                .Skip(offset * limit)
                .Take(limit);

            var pageData = _mapper.Map<List<ProductDtoBaseInfo>>(rawData);

            if (jwtId != null) CommonService.SetFlags(pageData, rawData, jwtId);

            return new Page<ProductDtoBaseInfo>()
            { CurrentOffset = offset, CurrentPage = pageData, Pages = pages, ElementsCount = elementsCount };


        }
        public async Task<List<ProductDtoBaseInfo>> GetRandom(HttpContext httpContext, int limit)
        {

            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);


            var idsFromCookies = LoadExceptProductsFromCookies(httpContext);
            var jwtId = await _commonService.UserIsOkNullable(httpContext);

            var rawResult = _context.Products
            .Include(x => x.Photos)
            .Include(x => x.Favourites)
            .Include(x => x.CartProducts)
            .Where(x => !idsFromCookies.Contains(x.Id))
            .OrderBy(x => EF.Functions.Random()).Take(limit);

            var result = _mapper.Map<List<ProductDtoBaseInfo>>(rawResult);

            if (jwtId != null) CommonService.SetFlags(result, rawResult, jwtId);

            return result;
        }
        public async Task<List<ProductDtoBaseInfo>> GetRandomPersonal(HttpContext httpContext, int limit)
        {
            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);

            var jwtId = await _commonService.UserIsOk(httpContext);

            var idsFromCookies = LoadExceptProductsFromCookies(httpContext);

            var favouriteCathegoriesIds = _context.Favourites.Include(x => x.Product).Where(x => x.UserId == jwtId).Select(x => x.Product!.CathegoryId).ToList();

            var rawResult = _context.Products.Where(x => favouriteCathegoriesIds.Contains(x.CathegoryId))
            .Include(x => x.Photos)
            .Include(x => x.Favourites)
            .Include(x => x.CartProducts)
            .Where(x => !idsFromCookies.Contains(x.Id))
            .OrderBy(x => EF.Functions.Random())
            .Take(limit);

            var result = _mapper.Map<List<ProductDtoBaseInfo>>(rawResult);

            CommonService.SetFlags(result, rawResult, jwtId);

            return result;
        }
        public async Task<List<ProductDtoBaseInfo>> GetRandomByCathegory(long cathegoryId, HttpContext httpContext, int limit)
        {

            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);


            var idsFromCookies = LoadExceptProductsFromCookies(httpContext);
            var jwtId = await _commonService.UserIsOkNullable(httpContext);

            var rawResult = _context.Products.Where(x => x.CathegoryId == cathegoryId)
            .Include(x => x.Photos)
            .Include(x => x.Favourites)
            .Include(x => x.CartProducts)
            .Where(x => !idsFromCookies.Contains(x.Id))
            .OrderBy(x => EF.Functions.Random())
            .Take(limit);

            var result = _mapper.Map<List<ProductDtoBaseInfo>>(rawResult);

            if (jwtId != null) CommonService.SetFlags(result, rawResult, jwtId);

            return result;
        }

        public async Task<Page<ProductDtoBaseInfo>> GetByCathegory(long cathegoryId ,int limit, int offset, HttpContext context)
        {

            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);

            var elementsCount = _context.Products.Where(x => x.CathegoryId == cathegoryId).Count();

            var pages = Page.CalculatePagesAmount(elementsCount, limit);
            if (pages <= 0) return new Page<ProductDtoBaseInfo>();
            if (!Page.OffsetIsOk(offset, pages)) throw new APIException($"Invalid offset: {offset}", 400);

            var jwtId = await _commonService.UserIsOkNullable(context);

            var rawData =
                _context.Products
                .Where(x => x.CathegoryId == cathegoryId)
                .Include(x => x.Photos)
                .Include(x => x.Favourites)
                .Include(x => x.CartProducts)
                .OrderByDescending(x => x.Purchases)
                .Skip(offset * limit)
                .Take(limit);

            var pageData = _mapper.Map<List<ProductDtoBaseInfo>>(rawData);

            if (jwtId != null) CommonService.SetFlags(pageData, rawData, jwtId);

            return new Page<ProductDtoBaseInfo>()
            { CurrentOffset = offset, CurrentPage = pageData, Pages = pages, ElementsCount = elementsCount };


        }

        private void FindAllParents(Cathegory? cathegory)
        {
            if (cathegory == null) return;
            cathegory.Parent = _context.Cathegories.Find(cathegory.ParentId);
            if (cathegory.Parent != null) FindAllParents(cathegory.Parent);
        }

        /* private static void SaveExceptProductToCookies(HttpContext context, long id)
        {
            //NB а что если куки наберется больше 4 кб?
            var idsFromCookies = new List<long>();

            if (context.Request.Cookies.ContainsKey("except_products"))
            {
                idsFromCookies = JsonSerializer.Deserialize<List<long>>(context.Request.Cookies["except_products"] ?? throw new Exception("can't deserialize cookie"))
                ?? throw new Exception("can't deserialize cookie");
            }

            idsFromCookies.Add(id);
            idsFromCookies = idsFromCookies.Distinct().ToList();

            context.Response.Cookies.Delete("except_products");
            context.Response.Cookies.Append("except_products", JsonSerializer.Serialize(idsFromCookies),
            new CookieOptions() { Secure = true, HttpOnly = true, MaxAge = new TimeSpan(365, 0, 0, 0) });
        } */

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