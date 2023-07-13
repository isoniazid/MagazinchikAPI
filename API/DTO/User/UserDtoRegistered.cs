namespace MagazinchikAPI.DTO.User
{
    public class UserDtoRegistered : IMapFrom<Model.User>
    {
        public long Id {get; set;}
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;   
        public string AccessToken {get; set;} = string.Empty;
    }
}