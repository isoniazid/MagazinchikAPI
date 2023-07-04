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
            return await _context.Products.ProjectTo<ProductDtoBaseInfo>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<ReviewDtoCreateResult> LeaveReview(ReviewDtoCreate input, HttpContext context)
        {
            var validation = _reviewCreateValidator.Validate(input);
            if (!validation.IsValid) throw new ValidatorException(validation);

            var jwtId = await UserIsOk(context);

            //Check if these things exist
            _ = await _context.Products.FindAsync(input.ProductId)
            ?? throw new APIException("Product does not exist", 404);

            var reviewToSave = _mapper.Map<Review>(input);
            reviewToSave.UserId = jwtId;

            await _context.Reviews.AddAsync(reviewToSave);
            await _context.SaveChangesAsync();

            return _mapper.Map<ReviewDtoCreateResult>(reviewToSave);

        }

        public async Task AddToFavourite(long productId, HttpContext context)
        {

            var jwtId = await UserIsOk(context);


            if(_context.Favourites.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId) != null)
            throw new APIException("Already added to favourites", 400);
            
            _ = await _context.Products.FindAsync(productId)
            ?? throw new APIException("Product does not exist", 404);


            var FavouriteToSave = new Favourite { ProductId = productId, UserId = jwtId};

            await _context.Favourites.AddAsync(FavouriteToSave);
            await _context.SaveChangesAsync();


        }

        private async Task<long> UserIsOk(HttpContext context)
        {
            long jwtId = Convert.ToInt64(context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new APIException("Broken acces token", 401));
            _ = await _context.Users.FindAsync(jwtId) ?? throw new APIException("Undefined user", 401);

            return jwtId;
        }
    }
}