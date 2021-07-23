namespace Web.Services.Products
{
    public class ProductServiceModel
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public decimal Price { get; init; }

        public string ImageUrl { get; init; }

        public bool InStock { get; init; }

        public string ManufacturerId { get; init; }
    }
}
