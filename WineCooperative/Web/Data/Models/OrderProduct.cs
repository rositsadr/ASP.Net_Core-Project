using System.ComponentModel.DataAnnotations;
using Web.Models;

namespace Web.Data.Models
{
    public class OrderProduct
    {
        [Required]
        public string OrderId { get; set; }

        public Order Order { get; set; }

        [Required]
        public string ProductId { get; set; }

        public Product Product { get; set; }
    }
}
