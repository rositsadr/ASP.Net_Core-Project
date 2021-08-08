using System;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class Service
    {
        [Key]
        [Required]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(ServiceMaxLength)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public string DateCreated { get; set; }

        public bool Available { get; set; }

        [Required]
        public string ManufacturerId { get; set; }

        public Manufacturer Manufacturer { get; set; }
    }
}
