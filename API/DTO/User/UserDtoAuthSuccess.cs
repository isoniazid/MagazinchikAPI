namespace MagazinchikAPI.DTO.User
{
    public class UserDtoAuthSuccess
    {
        public UserDtoAuthBaseInfo User {get; set;} = new();

        public string AccessToken {get; set;} = string.Empty;
    }
}