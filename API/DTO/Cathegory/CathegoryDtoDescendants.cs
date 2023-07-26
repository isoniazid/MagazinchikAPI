namespace MagazinchikAPI.DTO.Cathegory
{
    public class CathegoryDtoDescendants : IMapFrom<Model.Cathegory>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsParent {get; set;}
        public List<CathegoryDtoDescendants>? Descendants {get; set;}
    }
}