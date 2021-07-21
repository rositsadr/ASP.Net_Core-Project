namespace Web.Models.Manufacturers
{
    public class ManufacturerViewModel
    {
        public string Id { get; set; }

        public string Name { get; init; }

        public string Description { get; init; }

        public string Email { get; init; }

        public string PhoneNumber { get; init; }

        public ManufacturerAddressViewModel Address { get; init; }
    }
}
