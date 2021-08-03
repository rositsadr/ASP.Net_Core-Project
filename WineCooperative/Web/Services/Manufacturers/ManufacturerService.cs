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

    }
}
