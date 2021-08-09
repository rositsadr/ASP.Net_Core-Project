using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Models;
using Web.Services.Addresses;
using Web.Services.Manufacturers.Models;
using static Web.Services.Constants;

namespace Web.Services.Manufacturers
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly WineCooperativeDbContext data;
        private readonly IAddressService addressService;
        private readonly IConfigurationProvider config;

        public ManufacturerService(WineCooperativeDbContext data, IAddressService addressService, IMapper mapper)
        {
            this.data = data;
            this.addressService = addressService;
            this.config = mapper.ConfigurationProvider;
        }

        public IEnumerable<ManufacturerServiceModel> All() => data.Manufacturers
            .ProjectTo<ManufacturerServiceModel>(config)
            .ToList();

        public void Create(string name, string phoneNumber, string Email, string description, string street, string zipCode, string townName, string countryName, string userId)
        {
            var addressId = addressService.Address(street, townName, zipCode, countryName);

            var manufacturer = new Manufacturer
            {
                Name = name,
                PhoneNumber = phoneNumber,
                Email = Email,
                Description = description,
                AddressId = addressId,
                UserId = userId
            };

            data.Manufacturers.Add(manufacturer);
            data.SaveChanges();

        }

        public ManufacturerServiceModel Edit(string manufacturerId) => data.Manufacturers
                .Where(m => m.Id == manufacturerId)
                .ProjectTo<ManufacturerServiceModel>(config)
                .FirstOrDefault();

        public bool ApplyChanges(string manufacturerId, string name, string description, string phoneNumber, string email, string street, string townName, string zipCode)
        {
            var manufacturer = data.Manufacturers
                .Where(m => m.Id == manufacturerId)
                .FirstOrDefault();

            if (manufacturer == null)
            {
                return false;
            }

            var addressId = addressService.Address(street, townName, zipCode, CountryOfManufacturing);

            manufacturer.Name = name;
            manufacturer.Description = description;
            manufacturer.PhoneNumber = phoneNumber;
            manufacturer.Email = email;
            manufacturer.AddressId = addressId;

            data.SaveChanges();

            return true;
        }

        public ManufacturerServiceModel Details(string manufacturerId) => data.Manufacturers
                .Where(m => m.Id == manufacturerId)
                .ProjectTo<ManufacturerServiceModel>(config)
                .FirstOrDefault();

        public bool Delete(string manufacturerId)
        {
            var manufacturer = data.Manufacturers
                .Find(manufacturerId);

            if (manufacturer != null)
            {
                data.Manufacturers.Remove(manufacturer);
                data.SaveChanges();
                return true;
            }

            return false;
        }

        public bool ManufacturerExistsByName(string name) => data.Manufacturers
            .Any(m => m.Name == name);

        public bool ManufacturerExistsById(string manufacturerId) => this.data.Manufacturers
.Any(m => m.Id == manufacturerId);

        public IEnumerable<ManufacturerNameServiceModel> AllManufacturers() => this.GetManufacturers(data.Manufacturers);

        public IEnumerable<ManufacturerNameServiceModel> ManufacturersNameByUser(string userId) => this.GetManufacturers(data.Manufacturers.Where(m => m.UserId == userId));

        public IEnumerable<ManufacturerServiceModel> ManufacturersByUser(string userId) => data.Manufacturers
                .Where(m=>m.UserId == userId)
                .ProjectTo<ManufacturerServiceModel>(config)
                .ToList();

        private IEnumerable<ManufacturerNameServiceModel> GetManufacturers(IQueryable<Manufacturer> productQuery) => productQuery
            .ProjectTo<ManufacturerNameServiceModel>(config)
            .ToList();

        public bool IsUsersManufacturer(string userId, string manufacturerId) => data.Manufacturers
            .Any(m => m.Id == manufacturerId && m.UserId == userId);
    }
}
