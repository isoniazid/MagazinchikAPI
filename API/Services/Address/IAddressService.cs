using MagazinchikAPI.DTO.Address;

namespace MagazinchikAPI.Services.Address
{
    public interface IAddressService
    {
        public  Task<AddressDtoCreated> CreateAddress(AddressDtoCreate input, HttpContext httpContext);

        public Task<List<AddressDtoBaseInfo>> GetAllForUser(HttpContext context);
    }
}