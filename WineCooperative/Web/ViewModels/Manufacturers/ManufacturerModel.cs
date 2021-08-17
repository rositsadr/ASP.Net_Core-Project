using System.ComponentModel.DataAnnotations;
using Web.Services.Manufacturers.Models;
using static Web.Data.DataConstants;

namespace Web.ViewModels.Manufacturers
{
    public class ManufacturerModel
    {
        [Required]
        [StringLength(ManufacturerMaxLength, MinimumLength = ManufacturerMinLength)]
        public string Name { get; init; }

        public string Description { get; init; }

        [Required]
        [StringLength(ManufacturerPhoneNumberMaxLength, MinimumLength = ManufacturerPhoneNumberMinLength)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; init; }

        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [Display(Name ="Is functional")]
        public bool IsFunctional { get; init; }

        [Required]
        public ManufacturerAddressServiceModel Address { get; init; }
    }
}
