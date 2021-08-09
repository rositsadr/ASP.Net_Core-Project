using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models.Users
{
    public class AdditionalUserInfoAddingModel
    {
        [Required]
        [StringLength(NameMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} symbols long.", MinimumLength = NameMinLength)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(NameMaxLength, ErrorMessage = "The {0} must be at least {2} and at max {1} symbols long.", MinimumLength = NameMinLength)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        public AddressAddingModel Address { get; set; }
    }
}
