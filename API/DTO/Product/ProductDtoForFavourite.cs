namespace MagazinchikAPI.DTO.Product
{
    public class ProductDtoForFavourite : IMapFrom<Model.Product>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public long RateCount { get; set; }

        public float AverageRating { get; set; }

        public long Purchases { get; set; }

        public List<PhotoDtoProductBaseInfo>? Photos { get; set; }

        public bool IsFavourite { get; set; } = true;

        public bool IsInCart { get; set; } = false;


        public void SetCart(Model.Product product, long userId)
        {
            if (product.CartProducts == null) throw new Exception("Must be list");
            if (!product.CartProducts.Where(x => x.UserId == userId).IsNullOrEmpty())
                IsInCart = true;
        }
    }
}