namespace MagazinchikAPI.DTO.User
{
    public class UserDtoToken : IMapFrom<Model.User>
    {
        public string Email { get; set; } = string.Empty;
        public long Id {get; set;}

        public Model.UserRole Role {get; set;}
    }
}