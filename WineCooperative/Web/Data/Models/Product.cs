using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.Data.Models;
using static Web.Data.DataConstants;

namespace Web.Models
{
    public class Product
    {
        public Product()
        {
            this.ProductOrders = new HashSet<OrderProduct>();
            this.GrapeVarieties = new HashSet<ProductGrapeVariety>();
        }

        [Key]
        [Required]
        public string Id { get; init; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(ProductMaxLength)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int ManufactureYear { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public bool InStock { get; set; }

        public int ColorId { get; set; }

        public ProductColor Color { get; set; }

        public int TasteId { get; set; }

        public ProductTaste Taste { get; set; }

        [Required]
        public string ManufacturerId { get; set; }

        public Manufacturer Manufacturer { get; set; }

        [Required]
        public string WineAreaId { get; set; }

        public WineArea WineArea { get; set; }

        public ICollection<OrderProduct> ProductOrders { get; set; }
        public ICollection<ProductGrapeVariety> GrapeVarieties { get; set; }
    }
}