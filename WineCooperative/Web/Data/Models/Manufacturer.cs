using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class Manufacturer
    {
        public Manufacturer()
        {
            this.Products = new HashSet<Product>();
            this.Services = new HashSet<Service>();
        }

        [Key]
        [Required]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(ManufacturerMaxLength)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string AddressId { get; set; }

        public Address Address { get; set; }

        [Required]
        public string UserId { get; set; }

        public UserAdditionalInformation User { get; set; }

        public ICollection<Product> Products { get; set; }

        public ICollection<Service> Services { get; set; }
    }
}
