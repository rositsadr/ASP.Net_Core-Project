using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.Data.Models;

namespace Web.Models
{
    public class Order
    {
        public Order() => this.OrderProducts = new HashSet<OrderProduct>();

        [Key]
        [Required]
        public int Id { get; init; }

        public DateTime OrderDate { get; set; }

        [Required]
        public string UserId { get; set; }

        public UserAdditionalInformation User { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
