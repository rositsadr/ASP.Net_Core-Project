namespace Web.Services.Products.Models
{
    public class ProductServiceModel
    {
        public string Id { get; set; }

        public string Name { get; init; }

        public decimal Price { get; init; }

        public string ImageUrl { get; init; }

        public string ManufacturerId { get; init; }

        public bool InStock { get; init; }
    }
}
