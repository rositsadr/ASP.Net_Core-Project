using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.Data.Models;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class UserAdditionalInformation
    {
        public UserAdditionalInformation() => this.Orders = new HashSet<Order>();

        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(NameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string LastName { get; set; }

        [Required]
        public string AddressId { get; set; }

        public Address Address { get; set; }

        public ICollection<Order> Orders { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
