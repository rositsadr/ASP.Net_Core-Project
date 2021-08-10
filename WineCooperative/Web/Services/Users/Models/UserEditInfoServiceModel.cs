using Web.Services.Addresses;

namespace Web.Services.Users.Models
{
    public class UserEditInfoServiceModel
    {
        public string UserId { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string PhoneNumber { get; init; }

        public AddressEditServiceModel Address { get; init; }
    }
}
