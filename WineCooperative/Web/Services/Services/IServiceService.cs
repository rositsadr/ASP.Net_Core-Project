using System.Collections.Generic;
using Web.Models.Services;
using Web.Models.Services.Enums;
using Web.Services.Services.Models;

namespace Web.Services.Services
{
    public interface IServiceService
    {
        public void Create(string name, decimal price, string imageUrl, string description, string manufacturerId, bool available);

        public bool ServiceExists(string userId, string name);

        public IEnumerable<ServiceDetailsServiceModel> ServicesByUser(string userId);

        public ServiceSearchPageServiceModel All(int servicesPerRage, int currantPage, string searchTerm, ServiceSort sorting);
    }
}
