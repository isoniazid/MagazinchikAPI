namespace MagazinchikAPI.DTO.User
{
    public class UserDtoToken : IMapFrom<Model.User>
    {
        private string _email = String.Empty;
        public string Email { get => _email; set => _email = value.ToLower(); }
        public long Id {get; set;}

        public Model.UserRole Role {get; set;}
    }
}