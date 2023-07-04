namespace MagazinchikAPI.Model
{
    public class CartProduct : Traceable
    {
        public long Id {get; set;}
        public long ProductCount { get; set; }
        
        public long ProductId { get; set;}
        
        public Product Product {get; set;} = new();
    }
}