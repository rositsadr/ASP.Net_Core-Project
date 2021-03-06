using MyTested.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Data.Models;


namespace Web.Tests.Data
{
    public static class ManufacturerTestData
    {
        public static List<Manufacturer> GetManufacturers(int count, bool isFunctional = true)
        {
            var country = new Country { Id = 1, CountryName = "Bulgaria" };

            var manufacturers = Enumerable
                .Range(1, count)
                .Select(i => new Manufacturer
                {
                    Id = i.ToString(),
                    Name = $"Manufacturer {i}",
                    PhoneNumber = $"00359{i}{i + 1}{i + 2}{i + 3}",
                    IsFunctional = isFunctional,
                    Email = $"Manufacturer{i}@abv.bg",
                    UserId = TestUser.Identifier,
                    Address = new Address
                    {
                        Id = i,
                        Street = $"Street {i}",
                        Town = new Town
                        {
                            Name = $"Town{i}",
                            CountryId = 1,
                        }
                    }
                })
                .ToList();

            return manufacturers;
        }

        public static string ManufacturerId =>
            new Guid().ToString();

        public static Manufacturer GetManufacturerWithUser(string userMemberId, string manufacturerId) => new Manufacturer
        {
            Id = manufacturerId,
            Name = "TestManufacturer",
            UserId = userMemberId,
            IsFunctional = true,
        };

        public static Manufacturer ManufacturerWithFullData(string userId, string manufacturerId)
        {
            Manufacturer manufacturer = GetManufacturerWithUser(userId, manufacturerId);

            manufacturer.Email = "TestEmail";
            manufacturer.PhoneNumber = "TestPhoneNumber";
            manufacturer.Description = "TestDescription";
            manufacturer.Address = new Address
            {
                Street = "test",
                ZipCode = "test",                 
                Town = new Town
                {
                    Name = "test",
                    Id = 1
                },
            };

            return manufacturer;
        }
    }
}
