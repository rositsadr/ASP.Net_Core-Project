﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class GrapeVariety
    {
        public GrapeVariety()
        {
            this.WineAreas = new HashSet<WineArea>();
            this.Products = new HashSet<Product>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(GrapeVarietyMaxLength)]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<WineArea> WineAreas { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}