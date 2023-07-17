using System.Text.Json.Serialization;

namespace MagazinchikAPI.DTO
{
    public class PhotoDtoBannerBaseInfo : IMapFrom<Model.Photo>
    {
        public long Id { get; set; }

        [JsonPropertyName("order")]
        public int PhotoOrder { get; set; }
    }
}