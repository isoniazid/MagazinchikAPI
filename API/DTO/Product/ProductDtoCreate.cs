using System.Text.Json.Serialization;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.DTO
{
    public class ProductDtoCreate : IMapTo<Model.Product>
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public long CathegoryId {get; set;}
    }
}