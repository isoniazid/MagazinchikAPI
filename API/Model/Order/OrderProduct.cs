namespace MagazinchikAPI.Model
{
    public class OrderProduct
    {
        public long Id { get; set; }
        public Order Order { get; set; } = new();
        public long OrderId { get; set; }
        public Product Product {get; set;} = new();
        public long ProductId { get; set; }
        public long ProductCount { get; set; }

        public DateTime? CreatedAt {get; set;}
        public DateTime? UpdatedAt {get; set;}
    }
}