namespace MagazinchikAPI.Model
{
    public class Activation
    {
        public long Id { get; set; }
        public string Link { get; set; } = String.Empty;
        public bool IsActivated { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = new();
    }
}