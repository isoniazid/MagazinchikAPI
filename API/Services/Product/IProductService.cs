using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Review;

namespace MagazinchikAPI.Services
{
    public interface IProductService
    {
        public Task Create(ProductDtoCreate input);

        public Task<Page<ProductDtoBaseInfo>> GetAll(int limit, int offset, HttpContext context);

        public Task<ProductDtoDetailed> GetDetailedInfo(long productId, HttpContext httpContext);

        public Task AddToFavourite(long productId, HttpContext context);

        public Task RemoveFromFavourite(long productId, HttpContext context);

        public Task<List<ProductDtoBaseInfo>> GetRandomByCathegory(long cathegoryId, HttpContext httpContext, int limit);

        public Task<List<ProductDtoBaseInfo>> GetRandomPersonal(HttpContext httpContext, int limit);

        public Task<Page<ProductDtoBaseInfo>> GetPopular(int limit, int offset, HttpContext context);
    }
}