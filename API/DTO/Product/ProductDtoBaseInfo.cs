using System.Text.Json.Serialization;

namespace MagazinchikAPI.DTO
{
    public class ProductDtoBaseInfo :  IMapFrom<Model.Product>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public decimal Price { get; set; }

       // public long ReviewCount { get; set; }

        [JsonPropertyName("rateCount")]
        public long ReviewNoTextCount {get; set;}

        public float AverageRating { get; set; }

        public long Purchases { get; set; }

        //public CathegoryDtoBaseInfo? Cathegory {get; set;}

        public List<PhotoDtoProductBaseInfo>? Photos { get; set; }

        public bool IsFavourite {get; set;} = false;

        public bool IsInCart {get; set;} = false;
    }
}