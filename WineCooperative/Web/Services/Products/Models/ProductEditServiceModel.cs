using System.Collections.Generic;

namespace Web.Services.Products.Models
{
    public class ProductEditServiceModel : ProductServiceModel
    {
        public int ManufactureYear { get; init; }

        public string Description { get; init; }

        public bool InStock { get; init; }

        public int WineAreaId { get; init; }

        public IEnumerable<int> GrapeVarieties { get; init; }

        public int ColorId { get; init; }

        public int TasteId { get; init; }

        public string UserId { get; init; }

    }
}
