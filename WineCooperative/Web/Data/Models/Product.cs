﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class Product
    {
        public Product()
        {
            this.Orders = new HashSet<Order>();
            this.GrapeVarieties = new HashSet<GrapeVariety>();
        }

        [Key]
        [Required]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(ProductMaxLength)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public bool InStock { get; set; }

        [Required]
        public string ManufacturerId { get; set; }

        public Manufacturer Manufacturer { get; set; }

        [Required]
        public string WineAreaId { get; set; }

        public WineArea WineArea { get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<GrapeVariety> GrapeVarieties { get; set; }
    }
}