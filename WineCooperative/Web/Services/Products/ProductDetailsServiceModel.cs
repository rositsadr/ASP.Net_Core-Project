namespace Web.Services.Products
{
    public class ProductDetailsServiceModel
    {
        public string Id { get; set; }
        public string Name { get; init; }

        public decimal Price { get; init; }

        public int ManufactureYear { get; init; }

        public string ImageUrl { get; init; }

        public string Description { get; init; }

        public bool InStock { get; init; }

        public string ManufacturerName { get; init; }

        public string WineAreaName { get; init; }
    }
}
