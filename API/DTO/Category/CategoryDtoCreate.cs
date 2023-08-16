namespace MagazinchikAPI.DTO
{
    public class CategoryDtoCreate :IMapTo<Model.Category>
    {
        public string Name { get; set; } = string.Empty;

        public bool IsParent {get; set;}
        public long? ParentId { get; set; }
    }
}