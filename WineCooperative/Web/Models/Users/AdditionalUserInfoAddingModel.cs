using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Models.Users
{
    public class AdditionalUserInfoAddingModel
    {
        [Required]
        [StringLength(NameMaxLength, MinimumLength =NameMinLength)]
        public string FirstName { get; init; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string LastName { get; init; }

        [Required]
        public AddressAddingModel Address { get; init; }
    }
}
