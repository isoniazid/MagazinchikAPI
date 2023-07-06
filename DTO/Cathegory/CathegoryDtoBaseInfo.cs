using MagazinchikAPI.Model;

namespace MagazinchikAPI.DTO
{
    public class CathegoryDtoBaseInfo : IMapFrom<Cathegory>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CathegoryDtoBaseInfo? Parent { get; set; }
    }
}