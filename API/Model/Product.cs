namespace MagazinchikAPI.Model
{
    public class Product
    {
        public long Id { get; set; }

        private string _name = string.Empty;
        public string Name { get => _name; set => (_name, Slug) = (value, value.GenerateSlug()); }

        public string Slug { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public long RateCount { get; set; }
        public long ReviewCount { get; set; }

        public float AverageRating { get; set; }

        public List<OrderProduct>? OrderProducts { get; set; }

        public List<CartProduct>? CartProducts { get; set; }

        public List<Favourite>? Favourites { get; set; }

        public List<Review>? Reviews { get; set; }

        public List<Photo>? Photos { get; set; }

        public long Purchases { get; set; }

        public Category? Category { get; set; }
        public long? CategoryId { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}