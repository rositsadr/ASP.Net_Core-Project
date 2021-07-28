namespace Web.Services.Products.Models
{
    public class ProductDetailsServiceModel : ProductServiceModel
    {
        public int ManufactureYear { get; init; }

        public string Description { get; init; }

        public bool InStock { get; init; }

        public string Color { get; init; }

        public string Taste { get; init; }

        public string ManufacturerName { get; init; }

        public string WineAreaName { get; init; }
    }
}
