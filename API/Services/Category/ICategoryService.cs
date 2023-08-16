using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Category;

namespace MagazinchikAPI.Services
{
    public interface ICategoryService
    {
        public Task<CategoryDtoCreated> CreateCategory(CategoryDtoCreate input);

        public List<CategoryDtoBaseInfo> GetRandomCategories(int count);

        public Task<CategoryDtoDescendants> GetByIdDescendants(long categoryId);

        public Task<CategoryDtoBaseInfo> GetByIdParents(long categoryId);

        public Task<List<CategoryDtoDescendants>> GetAll();
    }
}