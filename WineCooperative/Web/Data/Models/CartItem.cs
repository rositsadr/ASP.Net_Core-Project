using System.ComponentModel.DataAnnotations;
using Web.Models;

namespace Web.Data.Models
{
    public class CartItem
    {
        [Required]
        public string UserId { get; set; }

        public int Quantity { get; set; }

        [Required]
        public string ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
