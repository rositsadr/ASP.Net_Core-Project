using System.Collections.Generic;
using Web.Services.Services.Models;

namespace Web.Services.Services
{
    public interface IServiceService
    {
        public void Create(string name, decimal price, string imageUrl, string description, string manufacturerId);

        public bool ServiceExists(string userId, string name);

        public IEnumerable<ServiceDetailsServiceModel> ServicesByUser(string userId);
    }
}
