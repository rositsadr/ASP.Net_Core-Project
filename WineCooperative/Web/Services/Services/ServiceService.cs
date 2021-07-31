using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Models;
using Web.Services.Services.Models;

namespace Web.Services.Services
{
    public class ServiceService:IServiceService
    {
        private readonly WineCooperativeDbContext data;

        public ServiceService(WineCooperativeDbContext data) => this.data = data;

        public void Create(string name, decimal price, string imageUrl, string description, string manufacturerId)
        {
            var serviceToAdd = new Service
            {
                Name = name,
                Price = price,
                ImageUrl = imageUrl,
                Description = description,
                ManufacturerId = manufacturerId
            };

            data.Services.Add(serviceToAdd);
            data.SaveChanges();
        }

        public bool ServiceExists(string userId, string name) => data.Services
            .Any(s => s.Manufacturer.UserId == userId && s.Name == name);

        public IEnumerable<ServiceDetailsServiceModel> ServicesByUser(string userId) => this.data.Services
            .Where(s => s.Manufacturer.UserId == userId)
            .Select(s => new ServiceDetailsServiceModel
            {
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                ManufacturerName = s.Manufacturer.Name,
                ImageUrl = s.ImageUrl,
            })
            .ToList();
    }
}
