namespace MagazinchikAPI.Model
{
    public class RefreshToken
    {
        public long Id {get; set;}
        public string Value { get; set; } = string.Empty;

        
        public long UserId { get; set; }

        
        public User User { get; set; } = new();

        private readonly DateTime Expires_;

        //NB peredelay!!!1111 
        public DateTime Expires { get => Expires_;
        init => Expires_ = DateTime.UtcNow + Starter.RefreshTokenTime; }
    }
}