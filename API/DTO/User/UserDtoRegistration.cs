using System.Text.Json.Serialization;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.DTO.User
{
    public class UserDtoRegistration : IMapTo<Model.User>
    {
        private string _email = String.Empty;
        public string Email { get => _email; set => _email = value.ToLower(); }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}