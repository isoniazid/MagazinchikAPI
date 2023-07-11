namespace MagazinchikAPI.DTO.Review
{
    public class ReviewDtoRateList
    {
        public float Average {get; set;}

        public List<int> Listing {get; set;} = new List<int>();
    }
}