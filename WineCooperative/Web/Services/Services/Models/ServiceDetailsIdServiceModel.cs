namespace Web.Services.Services.Models
{
    public class ServiceDetailsIdServiceModel : ServiceDetailsServiceModel
    {
        public string Id { get; init; }

        public string ManufacturerId { get; init; }

        public string DateCreated { get; init; }

        public bool Available { get; init; }

        public string UserId { get; init; }
    }
}
