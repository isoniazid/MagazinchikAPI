namespace MagazinchikAPI.Model
{
    public class Banner
    {
        public long Id {get; set;}
        public string Name {get; set;} = string.Empty;
        public List<Photo>? Photos { get; set; }
    }
}