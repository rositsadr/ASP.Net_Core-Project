using Web.Models;

namespace Web.Services.Addresses
{
    public interface IAddressService
    {
        public int Address(string street, string townName, string zipCode, string countryName);
    }
}
