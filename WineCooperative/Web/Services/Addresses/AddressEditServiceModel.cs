using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Services.Addresses
{
    public class AddressEditServiceModel
    {
        [Required]
        [Display(Name = "Address")]
        public string Street { get; init; }

        [Required]
        [StringLength(ZipCodeMaxLength, MinimumLength = ZipCodeMinLength)]
        public string ZipCode { get; init; }

        [Required]
        [StringLength(TownMaxLength, MinimumLength = TownMinLength)]
        [Display(Name = "Town")]
        public string TownName { get; init; }

        public string CountryName { get; init; }
    }
}
