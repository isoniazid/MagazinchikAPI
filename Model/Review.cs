namespace MagazinchikAPI.Model
{
    public class Review : Traceable
    {
        public long Id { get; set; }
        public long? UserId { get; set; }

        public User? User { get; set; }
        public long? ProductId { get; set; }

        public Product? Product { get; set; }
        public string? Text { get; set; }

        public float Rate { get; set; }
    }
}