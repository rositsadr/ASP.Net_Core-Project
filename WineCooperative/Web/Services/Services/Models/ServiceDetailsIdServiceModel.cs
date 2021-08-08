using System;

namespace Web.Services.Services.Models
{
    public class ServiceDetailsIdServiceModel : ServiceDetailsServiceModel
    {
        public string ManufacturerId { get; init; }

        public string DateCreated { get; init; }

        public string UserId { get; init; }
    }
}
