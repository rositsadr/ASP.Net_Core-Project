using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Web.Models;

namespace Web.Data.Models
{
    public class User : IdentityUser
    {
        public string UserDataId { get; set; }

        public UserAdditionalInformation UserData { get; set; }
    }
}
