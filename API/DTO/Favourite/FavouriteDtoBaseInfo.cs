using MagazinchikAPI.DTO.Product;

namespace MagazinchikAPI.DTO.Favourite
{
    public class FavouriteDtoBaseInfo : IMapFrom<Model.Favourite>
    {
        public long Id { get; set; }
        public ProductDtoForFavourite? Product { get; set; }
    }
}