using System.Collections.Generic;
using Web.Models.Services;
using Web.Models.Services.Enums;
using Web.Services.Services.Models;

namespace Web.Services.Services
{
    public interface IServiceService
    {
        public void Create(string name, decimal price, string imageUrl, string description, string manufacturerId, bool available);

        public ServiceSearchPageServiceModel All(int servicesPerRage, int currantPage, string searchTerm, ServiceSort sorting);

        public ServiceDetailsIdServiceModel Edit(string serviceId);

        public bool ApplyChanges(string serviceId, string name, string description, string imageUrl, decimal price, bool available, string manufacturerId);

        public ServiceDetailsIdServiceModel Details(string serviceId);

        public void Delete(string id);

        public bool ServiceExists(string manufacturerId, string name);

        public IEnumerable<ServiceDetailsServiceModel> ServicesByUser(string userId);

        public bool IsItUsersService(string userId, string productId);

    }
}
