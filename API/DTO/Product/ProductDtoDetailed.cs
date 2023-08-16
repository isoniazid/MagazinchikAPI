namespace MagazinchikAPI.DTO
{
    public class ProductDtoDetailed :  IMapFrom<Model.Product>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public long ReviewCount { get; set; }

        public long RateCount {get; set;}

        public float AverageRating { get; set; }

        public long Purchases { get; set; }

        public CathegoryDtoBaseInfo? Cathegory {get; set;}

        public List<PhotoDtoProductBaseInfo>? Photos { get; set; }

        public bool IsFavourite {get; set;} = false;

        public bool IsInCart {get; set;} = false;


        public void SetFlags(Model.Product product, long userId)
        {
            SetCart(product, userId);
            SetFavourite(product, userId);
        }

        public void SetCart(Model.Product product, long userId)
        {
            if (product.CartProducts is null) throw new Exception("Must be list");
            if (!product.CartProducts.Where(x => x.UserId == userId).IsNullOrEmpty())
                IsInCart = true;
        }

        public void SetFavourite(Model.Product product, long userId)
        {
            if (product.Favourites is null) throw new Exception("Must be empty list or null");

            if (!product.Favourites.Where(x => x.UserId == userId).IsNullOrEmpty())
                IsFavourite = true;
        }
    }
}