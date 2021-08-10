namespace Web.Services.Cart.Models
{
    public class CartServiceModel
    {
        public string CartItemId { get; init; }

        public string UserId { get; init; }

        public int Quantity { get; init; }

        public System.DateTime DateCreated { get; init; }

        public string ProductId { get; init; }
    }
}
