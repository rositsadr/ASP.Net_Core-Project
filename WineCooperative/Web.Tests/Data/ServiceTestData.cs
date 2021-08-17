using System;
using System.Collections.Generic;
using System.Linq;
using Web.Data.Models;

namespace Web.Tests.Data
{
    public static class ServiceTestData
    {
        public static List<Service> GetServices(string manufacturerId, string userId, int count, bool available = true)
        {
            var manufacturer = new Manufacturer()
            {
                Id = manufacturerId,
                UserId = userId,
                Name = $"Manufacturer{manufacturerId}",
            };

            var services = Enumerable
                .Range(1, count)
                .Select(i => new Service
                {
                    Id = i.ToString(),
                    Name = $"Service {i}",
                    ImageUrl = $"https://images.vivino.com/thumbs/i8zMkR-wQniSPG1QDvm8Ow_pb_600x60{i}.png",
                    Available = available,
                    Price = i,
                    ManufacturerId = manufacturerId,
                    Manufacturer = manufacturer,
                    Description = $"Test description {i}."
                })
                .ToList();

            return services;
        }

        public static string ServiceId =>
                new Guid().ToString();
    }
}
