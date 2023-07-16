using MagazinchikAPI.Model;

namespace MagazinchikAPI.DTO
{
    public class ProductDtoDetailed :  IMapFrom<Product>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public long ReviewCount { get; set; }

        public long ReviewNoTextCount {get; set;}

        public float AverageRating { get; set; }

        public long Purchases { get; set; }

        public CathegoryDtoBaseInfo? Cathegory {get; set;}

        public List<PhotoDtoProductBaseInfo>? Photos { get; set; }
    }
}