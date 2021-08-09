using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models.Users
{
    public class AddressAddingModel
    {
        [Required]
        public string Street { get; set; }

        [Required]
        [StringLength(TownMaxLength, MinimumLength =TownMinLength)]
        public string TownName { get; set; }

        [Required]
        [StringLength(ZipCodeMaxLength,MinimumLength =ZipCodeMinLength)]
        public string ZipCode { get; set; }

        [Required]
        [StringLength(CountryMaxLength, MinimumLength = CountryMinLength)]
        public string CountryName { get; set; }
    }
}
