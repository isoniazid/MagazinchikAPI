namespace MagazinchikAPI.DTO.Product
{
    public class ProductDtoForFavourite : IMapFrom<Model.Product>
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        //public CathegoryDtoBaseInfo? Cathegory { get; set; }
        public List<PhotoDtoProductBaseInfo>? Photos { get; set; }
    }
}