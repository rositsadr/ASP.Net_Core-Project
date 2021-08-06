namespace Web.Services.Cart.Models
{
    public class CartItemViewServiceModel
    {
        public string ProductId { get; init; }

        public string ProductName { get; init; }

        public decimal ProductPrice { get; init; }

        public string ProductImageUrl { get; init; }

        public string ProductManufacturerName { get; init; }

        public string UserId { get; init; }

        public int Quantity { get; set; }
    }
}
