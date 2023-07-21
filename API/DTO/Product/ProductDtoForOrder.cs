namespace MagazinchikAPI.DTO.Product
{
    public class ProductDtoForOrder : IMapFrom<Model.Product>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public List<PhotoDtoProductBaseInfo>? Photos { get; set; }

    }
}