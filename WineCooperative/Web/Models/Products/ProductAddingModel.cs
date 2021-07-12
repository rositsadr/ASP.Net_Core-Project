using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.Attributes;
using static Web.Data.DataConstants;

namespace Web.Models.Products
{
    public class ProductAddingModel
    {
        [Required]
        [MinLength(ProductMinLength)]
        [MaxLength(ProductMaxLength)]
        public string Name { get; init; }

        public decimal Price { get; init; }

        [Display(Name ="Image URL")]
        [Required]
        [Url]
        public string ImageUrl { get; init; }

        [Display(Name = "Year of manufacture")]
        [RangeUntilCurrentYear(ManufactureYearMinValue)]
        public int ManufactureYear { get; init; }

        public string Description { get; init; }

        [Display(Name = "In stock")]
        public bool InStock { get; init; }

        [Display(Name = "Wine Area")]
        [Required]
        public string WineAreaId { get; init; }

        [Display(Name = "Grape varieties:")]
        [Required]
        public ICollection<int> GrapeVarieties { get; init; }

        public IEnumerable<ProductWineAreaModel> WineAreas { get; set; }

        public IEnumerable<ProductGrapeVarieties> AllGrapeVarieties { get; set; }
    }
}
