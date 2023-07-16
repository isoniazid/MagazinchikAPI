using MagazinchikAPI.DTO.Banner;

namespace MagazinchikAPI.Services.Banner
{
    public interface IBannerService
    {
        public Task Create(BannerDtoCreate input);
    }
}