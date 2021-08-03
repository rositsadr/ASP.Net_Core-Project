namespace Web.Services.Services.Models
{
    public class ServiceDetailsServiceModel
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public string ImageUrl { get; init; }

        public decimal Price { get; init; }

        public string ManufacturerName { get; init; }

        public bool Available { get; init; }
    }
}
