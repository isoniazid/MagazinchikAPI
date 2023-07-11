namespace MagazinchikAPI.DTO.Review
{
    public class ReviewDtoCreateResult: IMapFrom<Model.Review>
    {
        public long ProductId { get; set; }
        public long UserId {get; set;}
        public string? Text { get; set; }
        public float Rate { get; set; }
        public DateTime UpdatedAt {get; set;}
        public DateTime CreatedAt{get; set;}
    }
}