using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Services
{
    public interface IProductService
    {
        public Task Create(ProductDtoCreate input);

        public Task<Page<ProductDtoBaseInfo>> GetAll(int limit, int offset, HttpContext context);

        public Task<ProductDtoDetailed> GetDetailedInfo(long productId, HttpContext httpContext);

        public Task<List<ProductDtoBaseInfo>> GetRandom(HttpContext context, int limit);

        public Task<List<ProductDtoBaseInfo>> GetRandomByCategory(long categoryId, HttpContext httpContext, int limit);

        public Task<List<ProductDtoBaseInfo>> GetRandomPersonal(HttpContext httpContext, int limit);

        public Task<Page<ProductDtoBaseInfo>> GetPopular(int limit, int offset, HttpContext context);

        public Task<Page<ProductDtoBaseInfo>> GetByCategory(long categoryId ,int limit, int offset, HttpContext context);
    }
}