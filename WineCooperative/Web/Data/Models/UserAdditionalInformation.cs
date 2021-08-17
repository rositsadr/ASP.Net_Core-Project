using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Data.Models
{
    public class UserAdditionalInformation
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(NameMaxLength)]
        public string LastName { get; set; }

        [Required]
        public int AddressId { get; set; }

        public Address Address { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }
    }
}
