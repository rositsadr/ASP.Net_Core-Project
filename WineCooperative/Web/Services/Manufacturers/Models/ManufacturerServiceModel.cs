namespace Web.Services.Manufacturers.Models
{
    public class ManufacturerServiceModel
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public string Email { get; init; }

        public string PhoneNumber { get; init; }

        public string Description { get; init; }

        public ManufacturerAddressServiceModel Address { get; init; }
    }
}
