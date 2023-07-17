using System.Text.Json.Serialization;

namespace MagazinchikAPI.DTO.Product
{
    public class ProductDtoForFavourite : IMapFrom<Model.Product>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        [JsonPropertyName("rateCount")]
        public long ReviewNoTextCount {get; set;}

        public float AverageRating { get; set; }

        public long Purchases { get; set; }

        public List<PhotoDtoProductBaseInfo>? Photos { get; set; }

        /* public bool IsFavourite {get; set;} = false;

        public bool IsInCart {get; set;} = false; */
    }
}