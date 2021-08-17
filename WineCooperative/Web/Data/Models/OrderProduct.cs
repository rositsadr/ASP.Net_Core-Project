using System.ComponentModel.DataAnnotations;

namespace Web.Data.Models
{
    public class OrderProduct
    {
        [Required]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        [Required]
        public string ProductId { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
