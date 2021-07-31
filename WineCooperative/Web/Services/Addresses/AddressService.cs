using System.Linq;
using Web.Data;
using Web.Models;

namespace Web.Services.Addresses
{
    public class AddressService : IAddressService
    {
        private readonly WineCooperativeDbContext data;

        public AddressService(WineCooperativeDbContext data)
        {
            this.data = data;
        }

        public int Address(string street, string townName, string zipCode, string countryName)
        {
            var townId = Town(townName, countryName);

            var address = data.Addresses
                .Where(a => a.Street == street && a.TownId == townId && a.ZipCode == zipCode)
                .FirstOrDefault();

            if(address == null)
            {
                address = new Address
                {
                    Street = street,
                    TownId = townId,
                    ZipCode = zipCode
                };

                data.Addresses.Add(address);
                data.SaveChanges();
            }

            return address.Id;
        }

        private int Town(string townName, string countryname)
        {
            var country = data.Countries.Where(c => c.CountryName == countryname).FirstOrDefault();

            if (country == null)
            {
                country = new Country {CountryName = countryname };

                data.Countries.Add(country);
            }

            var currantTown = data.Towns.Where(t => t.Name == townName && t.Country.Id == country.Id).FirstOrDefault();

            if (currantTown == null)
            {
                currantTown = new Town { Name = townName, Country = country };

                data.Towns.Add(currantTown);
            }

            data.SaveChanges();

            return currantTown.Id;
        }
    }
}
