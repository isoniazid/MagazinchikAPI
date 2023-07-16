namespace MagazinchikAPI.DTO.Banner
{
    public class BannerDtoCreate : IMapTo<Model.Banner>
    {
        public string Name {get; set;} = string.Empty;
    }
}