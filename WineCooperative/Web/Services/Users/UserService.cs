﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Data.Models;
using Web.Models;
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

        public UserEditInfoServiceModel Edit(string userId) => data.UserAdditionalInformation
                .Where(uai => uai.UserId == userId)
                .ProjectTo<UserEditInfoServiceModel>(config)
                .FirstOrDefault();

        public bool UserHasAdditionaInfo(string userId) => data.Users
            .Any(u => u.Id == userId && u.UserDataId != null);

        public bool UserApplyed(string userId) => data.Users
            .Any(u => u.Id == userId && u.Applyed);

        public void Apply(string userId)
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
    }
}
