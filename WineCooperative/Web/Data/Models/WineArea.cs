using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class WineArea
    {
        public WineArea()
        {
            this.Towns = new HashSet<Town>();
            this.Products = new HashSet<Product>();
            this.GrapeVarieties = new HashSet<GrapeVariety>();
        }

        [Key]
        [Required]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(WineAreaMaxLength)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public ICollection<Town> Towns { get; set; }

        public ICollection<Product> Products { get; set; }

        public ICollection<GrapeVariety> GrapeVarieties { get; set; }
    }
}
