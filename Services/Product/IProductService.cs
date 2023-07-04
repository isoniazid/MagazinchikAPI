using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Services
{
    public interface IProductService
    {
        public Task Create(ProductDtoCreate input);

        public Task<List<ProductDtoBaseInfo>> GetAll();

        public Task<DTO.Review.ReviewDtoCreateResult> LeaveReview(ReviewDtoCreate dto, HttpContext context);

        public Task AddToFavourite(long productId, HttpContext context);
    }
}