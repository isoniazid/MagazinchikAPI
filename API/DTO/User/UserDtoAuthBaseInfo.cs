namespace MagazinchikAPI.DTO.User
{
    public class UserDtoAuthBaseInfo : IMapFrom<Model.User>
    {
        
        public long Id {get; set;}

        private string _email = String.Empty;
        public string Email { get => _email; set => _email = value.ToLower(); }
        public string Name { get; set; } = string.Empty;   
        public Model.UserRole Role {get; set;}
    }
}