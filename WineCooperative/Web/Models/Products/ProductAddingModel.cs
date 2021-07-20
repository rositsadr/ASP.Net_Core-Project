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
        [StringLength(ProductMaxLength,MinimumLength = ProductMinLength)]
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
        public string WineAreaId { get; init; }

        [Display(Name = "Grape varieties:")]
        public IEnumerable<int> GrapeVarieties { get; init; }

        [Display(Name = "Manufacturer")]
        public string ManufacturerId { get; init; }

        public int ColorId { get; init; }

        public int TasteId { get; init; }

        public IEnumerable<ProductColorViewModel> AllColors { get; set; }

        public IEnumerable<ProductTasteViewModel> AllTastes { get; set; }

        public IEnumerable<ProductWineAreaModel> WineAreas { get; set; }

        public IEnumerable<ProductGrapeVarietiesModel> AllGrapeVarieties { get; set; }

        public IEnumerable<ProductManufacturerModel> Manufacturers { get; set; }
    }
}
