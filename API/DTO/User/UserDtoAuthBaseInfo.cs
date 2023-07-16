namespace MagazinchikAPI.DTO.User
{
    public class UserDtoAuthBaseInfo : IMapFrom<Model.User>
    {
        public long Id {get; set;}
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;   
        public Model.UserRole Role {get; set;}
    }
}