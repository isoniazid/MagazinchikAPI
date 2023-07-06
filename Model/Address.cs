namespace MagazinchikAPI.Model
{
    public class Address : Traceable
    {
        public long Id { get; set; }

        public string City { get; set; } = String.Empty;

        public string Street { get; set; } = String.Empty;

        public string House { get; set; } = String.Empty;

        public string Flat { get; set; } = String.Empty;

    }
}