using MagazinchikAPI.DTO.Product;

namespace MagazinchikAPI.DTO.CartProduct
{
    public class CartProductDtoBaseInfo : IMapFrom<Model.CartProduct>
    {
        public long Id { get; set; }
        public long ProductCount { get; set; }
        public ProductDtoForCart? Product { get; set; }
    }
}