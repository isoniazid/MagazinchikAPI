namespace MagazinchikAPI.DTO.Review
{
    public class ReviewDtoBaseInfo : IMapFrom<Model.Review>
    {
        public User.UserDtoReview? User {get; set;}
        public long Id {get; set;}
        public float Rate {get; set;}
        public string? Text {get; set;}
        public DateTime? UpdatedAt {get; set;}
    }
}