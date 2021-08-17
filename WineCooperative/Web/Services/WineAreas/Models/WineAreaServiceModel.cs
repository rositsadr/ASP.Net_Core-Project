using System.Collections.Generic;
using Web.Services.Manufacturers.Models;

namespace Web.Services.WineAreas.Models
{
    public class WineAreaServiceModel
    {
        public string Name { get; init; }

        public string Description { get; init; }

        public ICollection<ManufacturerNameServiceModel> Manufacturers { get; set; }

        public ICollection<WineAreasProductsServiceModel> Products { get; set; }
    }
}
