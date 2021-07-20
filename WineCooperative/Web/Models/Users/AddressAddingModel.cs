using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models.Users
{
    public class AddressAddingModel
    {
        [Required]
        public string Street { get; init; }

        [Required]
        [StringLength(TownMaxLength, MinimumLength =TownMinLength)]
        public string TownName { get; init; }

        [Required]
        [StringLength(ZipCodeMaxLength,MinimumLength =ZipCodeMinLength)]
        public string ZipCode { get; init; }

        [Required]
        [StringLength(CountryMaxLength, MinimumLength = CountryMinLength)]
        public string CountryName { get; init; }
    }
}
