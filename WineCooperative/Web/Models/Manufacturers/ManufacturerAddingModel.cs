using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models.Manufacturers
{
    public class ManufacturerAddingModel
    {
        [Required]
        [StringLength(ManufacturerMaxLength, MinimumLength = ManufacturerMinLength)]
        public string Name { get; init; }

        public string Description { get; init; }

        [Required]
        public ManufacturerAddressViewModel Address { get; init; }
    }
}
