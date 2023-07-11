using System.Text.Json.Serialization;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.DTO
{
    public class ReviewDtoCreate : IMapTo<Model.Review>
    {
        public long? ProductId { get; set; }
        public string? Text { get; set; }
        public float Rate { get; set; }
    }
}