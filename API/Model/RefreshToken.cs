namespace MagazinchikAPI.Model
{
    public class RefreshToken
    {
        public long Id { get; set; }
        public string Value { get; set; } = string.Empty;


        public long UserId { get; set; }


        public User User { get; set; } = new();

        public DateTime Expires { get; set; }
    }
}