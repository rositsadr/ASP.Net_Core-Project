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

        public ManufacturerService(WineCooperativeDbContext data, IAddressService addressService)
        {
            this.data = data;
            this.addressService = addressService;
        }

        public IEnumerable<ManufacturerServiceModel> All() => data.Manufacturers
                .Select(m => new ManufacturerServiceModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    PhoneNumber = m.PhoneNumber,
                    Description = m.Description,
                    Address = new ManufacturerAddressServiceModel
                    {
                        Street = m.Address.Street,
                        ZipCode = m.Address.ZipCode,
                        TownName = m.Address.Town.Name
                    }
                })
                .ToList();

        public void Create(string name, string phoneNumber, string Email, string description, string street, string zipCode, string townName, string countryName, string userId )
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

        public IEnumerable<ManufacturerByUserServiceModel> ManufacturersByUser(string userId)
        {
            var manufacturers = this.data.Manufacturers
                .Where(m => m.UserId == userId)
                .Select(m => new ManufacturerByUserServiceModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    UserId = m.UserId
                })
                .ToList();

            return manufacturers;
        }

    }
}
