using System.Text.Json.Serialization;

namespace MagazinchikAPI.DTO
{
    public class PhotoDtoProductBaseInfo : IMapFrom<Model.Photo>
    {
        public long Id { get; set; }

        [JsonPropertyName("order")]
        public int PhotoOrder { get; set; }
    }
}