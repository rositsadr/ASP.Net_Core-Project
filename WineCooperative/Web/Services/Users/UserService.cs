using System.Linq;
using Web.Data;
using Web.Models;
using Web.Services.Addresses;

namespace Web.Services.Users
{
    public class UserService : IUserService
    {
        private readonly WineCooperativeDbContext data;
        private readonly IAddressService addressService;

        public UserService(WineCooperativeDbContext data, IAddressService addressService)
        {
            this.data = data;
            this.addressService = addressService;
        }

        public bool UserIsManufacturer(string userId) => data.Manufacturers
            .Any(m => m.UserId == userId);

        public void AddUserAdditionalInfo(string userID, string firstName, string lastName, string street, string townName, string zipCode, string countryName)
        {
            var addressId = addressService.Address(street,townName,zipCode,countryName);

            var userInformation = new UserAdditionalInformation
            {
                FirstName = firstName,
                LastName = lastName,
                AddressId = addressId,
                UserId = userID,
            };

            data.UserAdditionalInformation.Add(userInformation);
            data.SaveChanges();
        }
    }
}
