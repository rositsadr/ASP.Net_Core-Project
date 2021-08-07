using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using Web.Models;

namespace Web.Data.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            this.Manufacturers = new HashSet<Manufacturer>();
            this.Orders = new HashSet<Order>();
        }

        public string UserDataId { get; set; }

        public UserAdditionalInformation UserData { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<Manufacturer> Manufacturers { get; set; }
    }
}
