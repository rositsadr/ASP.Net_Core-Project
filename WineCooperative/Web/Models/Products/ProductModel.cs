using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.Attributes;
using Web.Services.Manufacturers.Models;
using Web.Services.Products.Models;
using static Web.Data.DataConstants;

namespace Web.Models.Products
{
    public class ProductModel
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
        public int WineAreaId { get; init; }

        [Display(Name = "Grape varieties:")]
        public IEnumerable<int> GrapeVarieties { get; init; }

        [Display(Name = "Manufacturer")]
        [Required]
        public string ManufacturerId { get; init; }

        public int ColorId { get; init; }

        public int TasteId { get; init; }

        public IEnumerable<ProductColorServiceModel> AllColors { get; set; }

        public IEnumerable<ProductTasteServiceModel> AllTastes { get; set; }

        public IEnumerable<ProductWineAreaServiceModel> WineAreas { get; set; }

        public IEnumerable<ProductGrapeVarietiesServiceModel> AllGrapeVarieties { get; set; }

        public IEnumerable<ManufacturerByUserServiceModel> Manufacturers { get; set; }
    }
}
