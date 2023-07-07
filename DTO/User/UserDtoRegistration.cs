using System.Text.Json.Serialization;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.DTO.User
{
    public class UserDtoRegistration : IMapTo<Model.User>
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}