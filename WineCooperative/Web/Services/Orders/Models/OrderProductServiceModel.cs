namespace Web.Services.Orders.Models
{
    public class OrderProductServiceModel
    {
        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
