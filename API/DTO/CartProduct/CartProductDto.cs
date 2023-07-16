namespace MagazinchikAPI.DTO
{
    public class CartProductDto : IMapFrom<Model.Product>
    {
        public long ProductCount { get; set; }
        public long CartId { get; set; }
        public long ProductId { get; set; }
    }
}