using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Review;

namespace MagazinchikAPI.Services
{
    public interface IProductService
    {
        public Task Create(ProductDtoCreate input);

        public Task<List<ProductDtoBaseInfo>> GetAll();

        public Task<ProductDtoBaseInfo> GetBaseInfo(long productId);

        public Task<ReviewDtoCreateResult> LeaveReview(ReviewDtoCreate dto, HttpContext context);

        public Task<ReviewDtoCreateResult> UpdateReview(ReviewDtoUpdate input, HttpContext context);

        public Page<ReviewDtoBaseInfo> GetReviewsForProduct(long productId, int limit, int offset);

        public ReviewDtoRateList GetProductRateList(long productId);

        public Task AddToFavourite(long productId, HttpContext context);

        public Task RemoveFromFavourite(long productId, HttpContext context);

        public Task AddToCart(long productId, HttpContext context);

        public Task RemoveFromCart(long productId, HttpContext context);

        public Task DecreaseFromCart(long productId, HttpContext context);

        public List<ProductDtoBaseInfo> GetRandomByCathegory(long cathegoryId, HttpContext httpContext, int limit);

        public Task<List<ProductDtoBaseInfo>> GetRandomPersonal(HttpContext httpContext, int limit);

        public Page<ProductDtoBaseInfo> GetPopular(int limit, int offset);
    }
}