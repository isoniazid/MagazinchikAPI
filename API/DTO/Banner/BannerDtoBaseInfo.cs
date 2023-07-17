namespace MagazinchikAPI.DTO.Banner
{
    public class BannerDtoBaseInfo :IMapFrom<Model.Banner>
    {
        public long Id {get; set;}
        public string Name {get; set;} = string.Empty;
        public bool IsActive {get; set;}
        public List<PhotoDtoBannerBaseInfo>? Photos { get; set; }
    }
}