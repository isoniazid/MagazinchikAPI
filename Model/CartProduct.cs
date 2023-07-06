namespace MagazinchikAPI.Model
{
    public class CartProduct : Traceable
    {
        public long Id { get; set; }
        public long ProductCount { get; set; }

        public long? ProductId { get; set; }

        public User? User { get; set; }

        public long UserId { get; set; }

        public Product? Product { get; set; }
    }
}