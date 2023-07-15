namespace MagazinchikAPI.DTO.Address
{
    public class AddressDtoCreate :IMapTo<Model.Address>
    {
        public string City { get; set; } = String.Empty;

        public string Street { get; set; } = String.Empty;

        public string House { get; set; } = String.Empty;

        public string Flat { get; set; } = String.Empty;
    }
}