namespace MagazinchikAPI.Model
{
    public class Photo
    {
        public long Id {get; set;}
        public string FileName{get; set;} = string.Empty;
        public long ProductId{get; set;}

        public Product Product{get; set;} = new();
    }
}