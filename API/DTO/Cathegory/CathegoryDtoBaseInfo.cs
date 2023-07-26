namespace MagazinchikAPI.DTO
{
    public class CathegoryDtoBaseInfo : IMapFrom<Model.Cathegory>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsParent {get; set;}
        public CathegoryDtoBaseInfo? Parent { get; set; }
    }
}