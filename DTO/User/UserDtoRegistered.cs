namespace MagazinchikAPI.DTO.User
{
    public class UserDtoRegistered : IMapFrom<UserDtoRegistration>
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;   
        public string AccessToken {get; set;} = string.Empty;
    }
}