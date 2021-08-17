using System.ComponentModel.DataAnnotations;

namespace Web.Data.Models
{
    public class CartItem
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string ProductId { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
