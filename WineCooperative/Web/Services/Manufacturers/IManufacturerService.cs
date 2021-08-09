using System.Collections.Generic;
using Web.Services.Manufacturers.Models;

namespace Web.Services.Manufacturers
{
    public interface IManufacturerService
    {
        public IEnumerable<ManufacturerServiceModel> All();

        public void Create(string name, string phoneNumber, string Email, string description, string street, string zipCode, string townName, string countryName, string userId);

        public ManufacturerServiceModel Edit(string manufacturerId);

        public bool ApplyChanges(string manufacturerId, string name, string description, string phoneNumber, string email, string street, string townName, string zipCode);

        public ManufacturerServiceModel Details(string manufacturerId);

        public bool Delete(string manufacturerId);

        public bool ManufacturerExistsByName(string uniqueParameter);

        public bool ManufacturerExistsById(string manufacturerId);

        public IEnumerable<ManufacturerNameServiceModel> AllManufacturers();

        public IEnumerable<ManufacturerNameServiceModel> ManufacturersNameByUser(string userId);

        public IEnumerable<ManufacturerServiceModel> ManufacturersByUser(string userId);

        public bool IsUsersManufacturer(string userId, string manufacturerId);
    }
}
