using MagazinchikAPI.Model;

namespace MagazinchikAPI.DTO
{
    public class CathegoryDtoCreate :IMapTo<Cathegory>
    {
        public string Name { get; set; } = string.Empty;

        public bool IsParent {get; set;}
        public long? ParentId { get; set; }
    }
}