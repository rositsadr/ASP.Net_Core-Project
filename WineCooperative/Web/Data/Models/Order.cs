using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class Order
    {
        public Order()
        {
            this.Products = new HashSet<Product>();
        }

        [Key]
        [Required]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        public DateTime OrderDate { get; set; }

        [Required]
        public string UserId { get; set; }

        public UserAdditionalInformation User { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
