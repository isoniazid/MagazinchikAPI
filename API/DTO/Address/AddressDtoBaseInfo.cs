namespace MagazinchikAPI.DTO.Address
{
    public class AddressDtoBaseInfo : IMapFrom<Model.Address>
    {
        public long Id { get; set; }

        public string City { get; set; } = String.Empty;

        public string Street { get; set; } = String.Empty;

        public string House { get; set; } = String.Empty;

        public string Flat { get; set; } = String.Empty;

        public long? UserId {get; set;}
    }
}