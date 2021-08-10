using System.Collections.Generic;

namespace Web.Services.Orders.Models
{
    public class OrderServiceModel
    {
        public int OrderId { get; init; }

        public ICollection<OrderProductServiceModel> Products { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
