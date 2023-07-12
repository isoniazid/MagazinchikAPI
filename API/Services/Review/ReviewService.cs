using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Review;
using MagazinchikAPI.Infrastructure;
using FluentValidation;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.Services
{
    public class ReviewService : IReviewService
    {
        private const int LIMIT_SIZE = 50;

        private readonly ApplicationDbContext _context;
        private readonly IValidator<ReviewDtoCreate> _reviewCreateValidator;
        private readonly IValidator<ReviewDtoUpdate> _reviewUpdateValidator;
        private readonly IMapper _mapper;
        private readonly CommonService _commonService;
        public ReviewService(ApplicationDbContext context, CommonService commonService, IMapper mapper, IValidator<ReviewDtoCreate> reviewCreateValidator,
        IValidator<ReviewDtoUpdate> reviewUpdateValidator)
        {
            _commonService = commonService;
            _mapper = mapper;
            _context = context;
            _reviewCreateValidator = reviewCreateValidator;
            _reviewUpdateValidator = reviewUpdateValidator;
        }

        public async Task<ReviewDtoCreateResult> LeaveReview(ReviewDtoCreate input, HttpContext context)
        {
            var validation = _reviewCreateValidator.Validate(input);
            if (!validation.IsValid) throw new ValidatorException(validation);

            var jwtId = await _commonService.UserIsOk(context);

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

            var jwtId = await _commonService.UserIsOk(context);

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
                .Where(x => x.ProductId == productId && x.Text != null)
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
                Listing = new List<KeyValuePair<float, int>> {
            new (1.0f, reviews.Where(x => x.Rate == 1.0f).Count() ),
            new    ( 2.0f, reviews.Where(x => x.Rate == 2.0f).Count() ),
            new    ( 3.0f, reviews.Where(x => x.Rate == 3.0f).Count() ),
            new    ( 4.0f, reviews.Where(x => x.Rate == 4.0f).Count() ),
            new    ( 5.0f, reviews.Where(x => x.Rate == 5.0f).Count() )
            }
            };

            return result;
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
    }
}