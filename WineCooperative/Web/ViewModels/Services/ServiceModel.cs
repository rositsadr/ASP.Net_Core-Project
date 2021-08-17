using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.Services.Manufacturers.Models;
using static Web.Data.DataConstants;

namespace Web.ViewModels.Services
{
    public class ServiceModel
    {
        [Required]
        [StringLength(ServiceMaxLength,MinimumLength =ServiceMinLength)]
        public string Name { get; init; }

        [Required]
        public string Description { get; init; }

        [Display(Name = "Image URL")]
        [Url]
        public string ImageUrl { get; init; }

        public decimal Price { get; init; }

        public bool Available { get; init; }

        [Display(Name = "Manufacturer")]
        [Required]
        public string ManufacturerId { get; init; }

        public IEnumerable<ManufacturerNameServiceModel> Manufacturers { get; set; }
    }
}
