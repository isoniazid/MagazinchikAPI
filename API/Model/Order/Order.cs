namespace MagazinchikAPI.Model
{
    public class Order
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.AWAITING;
        public User? User { get; set; }
        public long? UserId { get; set; }

        public List<OrderProduct> OrderProducts = new();

        public Address? Address {get; set;}
        public long? AddressId { get; set; }

        public DateTime? CreatedAt {get; set;}
        public DateTime? UpdatedAt {get; set;}
    }
}