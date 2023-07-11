namespace MagazinchikAPI.Model
{
    public class Cathegory
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsParent {get; set;}

        public long? ParentId { get; set; }

        public Cathegory? Parent { get; set; }
    }
}