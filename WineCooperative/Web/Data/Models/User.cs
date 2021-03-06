using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Web.Data.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            this.Manufacturers = new HashSet<Manufacturer>();
            this.Orders = new HashSet<Order>();
        }

        public int? UserDataId { get; set; }

        public UserAdditionalInformation UserData { get; set; }

        public bool Applyed { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<Manufacturer> Manufacturers { get; set; }
    }
}
