using MagazinchikAPI.DTO.Product;

namespace MagazinchikAPI.DTO.Order
{
    public class OrderProductDtoBaseInfo : IMapFrom<Model.OrderProduct>
    {
        public long Id { get; set; }
        public ProductDtoForOrder? Product { get; set; }
        public long ProductCount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}