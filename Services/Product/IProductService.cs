using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Services
{
    public interface IProductService
    {
        public Task Create(ProductDtoCreate input);

        public Task<List<ProductDtoBaseInfo>> GetAll();
    }
}