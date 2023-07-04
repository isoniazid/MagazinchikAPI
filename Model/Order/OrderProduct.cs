namespace MagazinchikAPI.Model
{
    public class OrderProduct : Traceable
    {
        public long Id { get; set; }
        public Order Order { get; set; } = new();
        public long OrderId { get; set; }
        public Product Product {get; set;} = new();
        public long ProductId { get; set; }
        public long ProductCount { get; set; }
    }
}