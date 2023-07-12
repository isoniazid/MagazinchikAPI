namespace MagazinchikAPI.DTO.Review
{
    public class ReviewDtoRateList
    {
        public float Average {get; set;}

        public List<KeyValuePair<float, int>> Listing {get; set;} = new();
    }
}