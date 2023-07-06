using FluentValidation;
using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Review;
using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<ReviewDtoCreate> _reviewCreateValidator;
        public ProductService(ApplicationDbContext context, IMapper mapper, IValidator<ReviewDtoCreate> reviewCreateValidator)
        {
            _reviewCreateValidator = reviewCreateValidator;
            _context = context;
            _mapper = mapper;
        }
        public async Task Create(ProductDtoCreate input)
        {
            await _context.Products.AddAsync(_mapper.Map<Product>(input));
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductDtoBaseInfo>> GetAll()
        {
            var result = new List<ProductDtoBaseInfo>();
            var rawResult = await _context.Products.Include(x => x.Cathegory).ToListAsync();
            foreach (var element in rawResult)
            {
                FindAllParents(element.Cathegory);
                result.Add(_mapper.Map<ProductDtoBaseInfo>(element));
            }
            return result;

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

            await _context.Reviews.AddAsync(reviewToSave);
            await RecomputeProductRate(input.ProductId);
            
            //Savechanges is called in RecomputeProductRate
            //await _context.SaveChangesAsync();

            return _mapper.Map<ReviewDtoCreateResult>(reviewToSave);

        }


        public async Task<ReviewDtoCreateResult> UpdateReview(ReviewDtoCreate input, HttpContext context)
        {
            var validation = _reviewCreateValidator.Validate(input);
            if (!validation.IsValid) throw new ValidatorException(validation);

            var jwtId = await UserIsOk(context);

            //Check if review was already created
            var reviewToUpdate = _context.Reviews.Include(x => x.Product).FirstOrDefault(x => x.UserId == jwtId && x.ProductId == input.ProductId)
            ?? throw new APIException("Can't find review to update", StatusCodes.Status404NotFound);

            reviewToUpdate.UpdateTime();
            reviewToUpdate.Rate = input.Rate;
            reviewToUpdate.Text = input.Text;

            await _context.SaveChangesAsync();

            await RecomputeProductRate(input.ProductId);

            return _mapper.Map<ReviewDtoCreateResult>(reviewToUpdate);

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
    }
}