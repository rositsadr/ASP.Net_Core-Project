using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Data.Models;
using Web.Services.Addresses;
using Web.Services.Users.Models;


namespace Web.Services.Users
{
    public class UserService : IUserService
    {

        private readonly WineCooperativeDbContext data;
        private readonly IAddressService addressService;
        private readonly IConfigurationProvider config;

        public UserService(WineCooperativeDbContext data, IAddressService addressService, IMapper mapper)
        {
            this.data = data;
            this.addressService = addressService;
            this.config = mapper.ConfigurationProvider;
        }

        public IEnumerable<UserInfoServiceModel> All() => this.GetUsers(data.Users);

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

        public UserEditInfoServiceModel Edit(string userId) => data.UserAdditionalInformation
                .Where(uai => uai.UserId == userId)
                .ProjectTo<UserEditInfoServiceModel>(config)
                .FirstOrDefault();

        public bool ApplyChanges(string userId, string firstName, string lastName, string street, string townName, string zipCode, string countryName)
        {
            var info = data.UserAdditionalInformation
                .Where(ua => ua.UserId == userId)
                .FirstOrDefault();

            if (info == null)
            {
                return false;
            }

            info.FirstName = firstName;
            info.LastName = lastName;

            var addressId = addressService.Address(street, townName, zipCode, countryName);

            info.AddressId = addressId;

           data.SaveChanges();

            return true;
        }

        public bool UserHasAdditionaInfo(string userId) => data.Users
            .Any(u => u.Id == userId && u.UserDataId != null);

        public bool UserApplyed(string userId) => data.Users
            .Any(u => u.Id == userId && u.Applyed);

        public void ApplyForMember(string userId)
        {
            var user = data.Users
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            user.Applyed = true;
            data.SaveChanges();
        }

        public void NotApplyed(string userId)
        {
            var user = data.Users
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            user.Applyed = false;
            data.SaveChanges();
        }

        private IEnumerable<UserInfoServiceModel> GetUsers(IQueryable<User> userQuery)=> userQuery
            .ProjectTo<UserInfoServiceModel>(config)
            .ToList();

        public bool UserExists(string userId) => data.Users
            .Any(u => u.Id == userId);

        public UserInfoServiceModel GetUserWithData(string userId) => data.Users
                .Where(u => u.Id == userId)
                .ProjectTo<UserInfoServiceModel>(config)
                .FirstOrDefault();

        public void ChangeAllUsersProductsToNotInStock(string userId)
        {
            var products = data.Products
                .Where(p => p.Manufacturer.UserId == userId)
                .ToList();

            foreach (var product in products)
            {
                product.InStock = false;
            }

            data.SaveChanges();
        }

        public void ChangeAllServiceToNotAvailable(string userId)
        {
            var services = data.Services
                .Where(s => s.Manufacturer.UserId == userId)
                .ToList();

            foreach (var service in services)
            {
                service.Available = false;
            }

            data.SaveChanges();
        }

        public void ChangeAllManufacturersToNotFunctional(string userId)
        {
            var manufacturers = data.Manufacturers
                .Where(m => m.UserId == userId)
                .ToList();

            foreach (var manufacturer in manufacturers)
            {
                manufacturer.IsFunctional = false;
            }

            data.SaveChanges();
        }
    }
}
