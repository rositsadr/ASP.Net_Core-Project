using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Services.Users.Models
{
    public class UserInfoServiceModel
    {
        public string Id { get; init; }

        public string Username { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string  Email { get; init; }

        public bool Applyed { get; init; }
    }
}
