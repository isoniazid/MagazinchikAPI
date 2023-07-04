namespace MagazinchikAPI.Model
{
    public class Favourite : Traceable
    {
        public long Id { get; set; }
        public User? User {get; set;}
        public long? UserId { get; set;}
        public long? ProductId { get; set;}
        
        public Product? Product {get; set;}


    }
}