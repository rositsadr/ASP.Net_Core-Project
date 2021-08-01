using System.Collections.Generic;
using Web.Services.Manufacturers.Models;

namespace Web.Services.Manufacturers
{
    public interface IManufacturerService
    {
        public IEnumerable<ManufacturerServiceModel> All();

        public void Create(string name, string phoneNumber, string Email, string description, string street, string zipCode, string townName, string countryName, string userId);

        public bool ManufacturerExistsByName(string uniqueParameter);

        public bool ManufacturerExistsById(string manufacturerId);

        public IEnumerable<ManufacturerNameServiceModel> AllManufacturers();

        public IEnumerable<ManufacturerNameServiceModel> ManufacturersByUser(string userId);
    }
}
