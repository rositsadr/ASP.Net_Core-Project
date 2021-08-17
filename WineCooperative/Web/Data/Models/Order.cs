using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Data.Models
{
    public class Order
    {
        public Order() => this.OrderProducts = new HashSet<OrderProduct>();

        [Key]
        [Required]
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
