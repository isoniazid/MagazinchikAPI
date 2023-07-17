using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Favourite;

namespace MagazinchikAPI.Services.Favourite
{
    public interface IFavouriteService
    {
        public Task AddToFavourite(long productId, HttpContext context);

        public Task RemoveFromFavourite(long productId, HttpContext context);

        public Task<Page<FavouriteDtoBaseInfo>> GetAllFavouritesForUser(HttpContext context, int limit, int offset);
    }
}