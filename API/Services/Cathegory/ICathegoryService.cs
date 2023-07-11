using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Services
{
    public interface ICathegoryService
    {
        public Task<CathegoryDtoCreated> CreateCathegory(CathegoryDtoCreate input);

        public List<CathegoryDtoBaseInfo> GetRandomCathegories(int count);
    }
}