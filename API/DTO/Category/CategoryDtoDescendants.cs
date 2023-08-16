namespace MagazinchikAPI.DTO.Category
{
    public class CategoryDtoDescendants : IMapFrom<Model.Category>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsParent {get; set;}
        public List<CategoryDtoDescendants>? Descendants {get; set;}
    }
}