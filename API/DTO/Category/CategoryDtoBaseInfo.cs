namespace MagazinchikAPI.DTO
{
    public class CategoryDtoBaseInfo : IMapFrom<Model.Category>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsParent {get; set;}
        public CategoryDtoBaseInfo? Parent { get; set; }
    }
}