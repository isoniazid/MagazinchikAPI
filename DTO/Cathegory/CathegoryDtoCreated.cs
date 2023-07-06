namespace MagazinchikAPI.DTO
{
    public class CathegoryDtoCreated : IMapFrom<Model.Cathegory>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CathegoryDtoBaseInfo? Parent { get; set; }
    }
}