using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Web.Data.DataConstants;

namespace Web.Data.Models
{
    public class Address
    {
        public Address()
        {
            this.Users = new HashSet<UserAdditionalInformation>();
            this.Manufacturers = new HashSet<Manufacturer>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(AddressMaxLength)]
        public string Street { get; set; }

        [Required]
        [MaxLength(ZipCodeMaxLength)]
        public string ZipCode { get; set; }

        public int TownId { get; set; }

        public Town Town { get; set; }

        public ICollection<UserAdditionalInformation> Users { get; set; }

        public ICollection<Manufacturer> Manufacturers { get; set; }

    }
}
