using System.ComponentModel.DataAnnotations.Schema;

namespace MagazinchikAPI.Model
{
    public class Category
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsParent {get; set;}

        public long? ParentId { get; set; }

        public Category? Parent { get; set; }

        [NotMapped]
        public List<Category>? Descendants {get; set;}
    }
}