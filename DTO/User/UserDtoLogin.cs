namespace MagazinchikAPI.DTO.User
{
    public class UserDtoLogin
    {
        public string Email { get; set; } = string.Empty;

        private string _password = string.Empty;
        public string Password { get => _password; set => _password = HashPassword(value, Email); } 




        public string HashPassword(string password, string email)//Я не стал генерировать соль, потому что емейл у пользователя уже уникальный. Просто суммирую пароль и емейл и смотрю, чтобы все сошлось
        {
            var encrypter = new System.Security.Cryptography.HMACSHA256 { Key = Starter.passwordHashKey };
            return Convert.ToBase64String(encrypter.ComputeHash(Encoding.UTF8.GetBytes(password + email)))
            ?? throw new Exception("Incorrect values for HashPassword");
        }
    }


}