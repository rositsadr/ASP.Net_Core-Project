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
        [StringLength(ManufacturerPhoneNumberMaxLength, MinimumLength = ManufacturerPhoneNumberMinLength)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public ManufacturerAddressViewModel Address { get; init; }
    }
}
