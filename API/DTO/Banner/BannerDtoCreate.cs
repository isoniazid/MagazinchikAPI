namespace MagazinchikAPI.DTO.Banner
{
    public class BannerDtoCreate : IMapTo<Model.Banner>
    {
        public string Name {get; set;} = string.Empty;

        public bool IsActive {get; set;} = false;
    }
}