namespace MagazinchikAPI.DTO
{
    public class Page<T>
    {
        public int Pages {get; set;}
        public int CurrentOffset {get; set;}

        public List<T> CurrentPage {get; set;} = new();
    }
}