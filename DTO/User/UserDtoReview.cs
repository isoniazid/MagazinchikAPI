namespace MagazinchikAPI.DTO.User
{
    public class UserDtoReview : IMapFrom<Model.User>
    {
        public long Id {get; set;}
        public string Name {get; set;} = string.Empty;
    }
}