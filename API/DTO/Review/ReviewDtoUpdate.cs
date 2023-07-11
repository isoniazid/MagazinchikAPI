using System.Text.Json.Serialization;

namespace MagazinchikAPI.DTO
{
    public class ReviewDtoUpdate : IMapTo<Model.Review>
    {
        public long? ProductId { get; set; }
        public string? Text { get; set; }
        public float Rate { get; set; }
    }
}