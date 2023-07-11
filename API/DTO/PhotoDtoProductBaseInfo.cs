namespace MagazinchikAPI.DTO
{
    public class PhotoDtoProductBaseInfo : IMapFrom<Model.Photo>
    {
        public long Id { get; set; }
        public int PhotoOrder { get; set; }
    }
}