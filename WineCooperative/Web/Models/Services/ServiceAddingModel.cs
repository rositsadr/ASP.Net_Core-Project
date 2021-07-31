using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Web.Services.Manufacturers.Models;
using static Web.Data.DataConstants;

namespace Web.Models.Services
{
    public class ServiceAddingModel
    {
        [Required]
        [StringLength(ServiceMaxLength,MinimumLength =ServiceMinLength)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Image URL")]
        [Url]
        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        [Display(Name = "Manufacturer")]
        [Required]
        public string ManufacturerId { get; init; }

        public IEnumerable<ManufacturerByUserServiceModel> Manufacturers { get; set; }
    }
}
