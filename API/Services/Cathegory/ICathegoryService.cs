using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Cathegory;

namespace MagazinchikAPI.Services
{
    public interface ICathegoryService
    {
        public Task<CathegoryDtoCreated> CreateCathegory(CathegoryDtoCreate input);

        public List<CathegoryDtoBaseInfo> GetRandomCathegories(int count);

        public Task<CathegoryDtoDescendants> GetById(long cathegoryId);

        public Task<List<CathegoryDtoDescendants>> GetAll();
    }
}