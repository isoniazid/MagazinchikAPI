using MagazinchikAPI.DTO.Address;

namespace MagazinchikAPI.DTO.Order
{
    public class OrderDtoBaseInfo : IMapFrom<Model.Order>
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public Model.OrderStatus OrderStatus { get; set; }
        public List<OrderProductDtoBaseInfo>? OrderProducts {get; set;}
        public AddressDtoBaseInfo? Address {get; set;}
        public DateTime? CreatedAt {get; set;}
        public DateTime? UpdatedAt {get; set;}
        public string? PaymentId {get; set;}
    }
}