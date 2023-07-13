using System.Reflection.Metadata.Ecma335;

namespace MagazinchikAPI.Model
{
    public class User
    {
        public long Id { get; set; }


        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.USER;
        public string Name { get; set; } = string.Empty;

        private string _password = string.Empty;
        public string Password { get => _password; set => _password = HashPassword(value, Email); }
        public List<Order>? Orders { get; set; }
        public List<CartProduct>? CartProducts { get; set; }
        public List<Favourite>? Favourites { get; set; }
        public List<Review>? Reviews { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }

        public List<Address>? Addresses {get; set;}

        public DateTime? CreatedAt {get; set;}
        public DateTime? UpdatedAt {get; set;}

        public string HashPassword(string password, string email)//Я не стал генерировать соль, потому что емейл у пользователя уже уникальный. Просто суммирую пароль и емейл и смотрю, чтобы все сошлось
        {
            var encrypter = new System.Security.Cryptography.HMACSHA256 { Key = Starter.PASSWORD_HASH_KEY };
            return Convert.ToBase64String(encrypter.ComputeHash(Encoding.UTF8.GetBytes(password + email)))
            ?? throw new Exception("Incorrect values for HashPassword");
        }
    }
}