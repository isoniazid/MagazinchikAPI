using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Review;

namespace MagazinchikAPI.Services
{
    public interface IReviewService
    {
        public Task<ReviewDtoCreateResult> LeaveReview(ReviewDtoCreate dto, HttpContext context);

        public Task<ReviewDtoCreateResult> UpdateReview(ReviewDtoUpdate input, HttpContext context);

        public Page<ReviewDtoBaseInfo> GetReviewsForProduct(long productId, int limit, int offset);

        public ReviewDtoRateList GetProductRateList(long productId);
    }
}