namespace MagazinchikAPI.DTO.User
{
    public class UserDtoLogin
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set;} =string.Empty;




        public string HashPassword(string password, string email)//Я не стал генерировать соль, потому что емейл у пользователя уже уникальный. Просто суммирую пароль и емейл и смотрю, чтобы все сошлось
        {
            var encrypter = new System.Security.Cryptography.HMACSHA256 { Key = Starter.PASSWORD_HASH_KEY };
            return Convert.ToBase64String(encrypter.ComputeHash(Encoding.UTF8.GetBytes(password + email)))
            ?? throw new Exception("Incorrect values for HashPassword");
        }
    }


}