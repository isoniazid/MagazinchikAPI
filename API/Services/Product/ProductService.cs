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
        private readonly IValidator<ReviewDtoCreate> _reviewCreateValidator;
        private readonly IValidator<ReviewDtoUpdate> _reviewUpdateValidator;
        public ProductService(ApplicationDbContext context, IMapper mapper, IValidator<ReviewDtoCreate> reviewCreateValidator,
        IValidator<ReviewDtoUpdate> reviewUpdateValidator)
        {
            _reviewCreateValidator = reviewCreateValidator;
            _reviewUpdateValidator = reviewUpdateValidator;
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

            var jwtId = await UserIsOk(httpContext);

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

        public async Task<ReviewDtoCreateResult> LeaveReview(ReviewDtoCreate input, HttpContext context)
        {
            var validation = _reviewCreateValidator.Validate(input);
            if (!validation.IsValid) throw new ValidatorException(validation);

            var jwtId = await UserIsOk(context);

            //Check if these things exist
            var productToReview = await _context.Products.FindAsync(input.ProductId)
            ?? throw new APIException("Product does not exist", 404);
            productToReview.ReviewCount++;

            //Check if review was already created
            if (_context.Reviews.FirstOrDefault(x => x.UserId == jwtId && x.ProductId == input.ProductId) != null)
                throw new APIException("Review for this product was already left", StatusCodes.Status400BadRequest);

            var reviewToSave = _mapper.Map<Review>(input);
            reviewToSave.UserId = jwtId;
            (reviewToSave.UpdatedAt, reviewToSave.CreatedAt) = (DateTime.UtcNow, DateTime.UtcNow);

            await _context.Reviews.AddAsync(reviewToSave);

            await _context.SaveChangesAsync();

            await RecomputeProductRate(input.ProductId);

            return _mapper.Map<ReviewDtoCreateResult>(reviewToSave);

        }

        public async Task<ReviewDtoCreateResult> UpdateReview(ReviewDtoUpdate input, HttpContext context)
        {
            var validation = _reviewUpdateValidator.Validate(input);
            if (!validation.IsValid) throw new ValidatorException(validation);

            var jwtId = await UserIsOk(context);

            //Check if review was already created
            var reviewToUpdate = _context.Reviews.Include(x => x.Product).FirstOrDefault(x => x.UserId == jwtId && x.ProductId == input.ProductId)
            ?? throw new APIException("Can't find review to update", StatusCodes.Status404NotFound);

            //reviewToUpdate.UpdateTime();
            reviewToUpdate.UpdatedAt = DateTime.UtcNow;
            reviewToUpdate.Rate = input.Rate;
            reviewToUpdate.Text = input.Text;

            await _context.SaveChangesAsync();

            await RecomputeProductRate(input.ProductId);

            return _mapper.Map<ReviewDtoCreateResult>(reviewToUpdate);

        }

        public Page<ReviewDtoBaseInfo> GetReviewsForProduct(long productId, int limit, int offset)
        {
            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);

            var pages = (int)Math.Ceiling((float)_context.Reviews.Where(x => x.ProductId == productId).Count() / (float)limit);
            if (offset > pages - 1 || offset < 0) throw new APIException($"Invalid offset: {offset}", 400);

            var pageData = _mapper.Map<List<ReviewDtoBaseInfo>>(
                _context.Reviews.Include(x => x.User)
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.UpdatedAt)
                .Skip(offset * limit)
                .Take(limit));

            return new Page<ReviewDtoBaseInfo>() { CurrentOffset = offset, CurrentPage = pageData, Pages = pages };
        }

        public ReviewDtoRateList GetProductRateList(long productId)
        {
            var product = _context.Products.Find(productId)
            ?? throw new APIException("No such product", 404);

            var reviews = _context.Reviews.Where(x => x.ProductId == productId);


            var averageRating = product.AverageRating;


            var result = new ReviewDtoRateList
            {
                Average = averageRating,
                Listing = new List<int>(){
            reviews.Where(x => x.Rate == 1.0f).Count(),
            reviews.Where(x => x.Rate == 2.0f).Count(),
            reviews.Where(x => x.Rate == 3.0f).Count(),
            reviews.Where(x => x.Rate == 4.0f).Count(),
            reviews.Where(x => x.Rate == 5.0f).Count(),
            }
            };

            return result;
        }

        public async Task AddToFavourite(long productId, HttpContext context)
        {

            var jwtId = await UserIsOk(context);


            if (_context.Favourites.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId) != null)
                throw new APIException("Already added to favourites", 400);

            _ = await _context.Products.FindAsync(productId)
            ?? throw new APIException("Product does not exist", 404);


            var FavouriteToSave = new Favourite { ProductId = productId, UserId = jwtId };

            await _context.Favourites.AddAsync(FavouriteToSave);
            await _context.SaveChangesAsync();
        }

        public async Task AddToCart(long productId, HttpContext context)
        {
            var jwtId = await UserIsOk(context);


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
            var jwtId = await UserIsOk(context);

            var productToRemove = _context.CartProducts.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId)
            ?? throw new APIException("Nothing to remove", 404);

            _context.CartProducts.Remove(productToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task DecreaseFromCart(long productId, HttpContext context)
        {
            var jwtId = await UserIsOk(context);


            var productToDecrease = _context.CartProducts.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId)
            ?? throw new APIException("Nothing to decrease", 404);

            if (productToDecrease.ProductCount != 0) productToDecrease.ProductCount--;
            else _context.CartProducts.Remove(productToDecrease);


            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromFavourite(long productId, HttpContext context)
        {
            var jwtId = await UserIsOk(context);

            var favouriteToRemove = _context.Favourites.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId)
            ?? throw new APIException("Nothing to remove", 404);

            _context.Favourites.Remove(favouriteToRemove);
            await _context.SaveChangesAsync();
        }

        private async Task RecomputeProductRate(long? productId)
        {
            var product = await _context.Products.FindAsync(productId)
            ?? throw new APIException("No Such product", 404);
            var rates = _context.Reviews.Where(x => x.ProductId == productId).Select(x => x.Rate);

            float totalRate = 0f;

            foreach (var rate in rates)
            {
                totalRate += rate;
            }
            product.AverageRating = totalRate / product.ReviewCount;

            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private async Task<long> UserIsOk(HttpContext context)
        {
            long jwtId = Convert.ToInt64(context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new APIException("Broken acces token", 401));
            _ = await _context.Users.FindAsync(jwtId) ?? throw new APIException("Undefined user", 401);

            return jwtId;
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