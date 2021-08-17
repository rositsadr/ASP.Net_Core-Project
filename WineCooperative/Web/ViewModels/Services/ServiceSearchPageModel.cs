using System.Collections.Generic;
using Web.ViewModels.Services.Enums;
using Web.Services.Services.Models;

namespace Web.ViewModels.Services
{
    public class ServiceSearchPageModel
    {
        public const int servicesPerPage = 3;

        public int CurrantPage { get; init; } = 1;

        public int TotalServices { get; set; }

        public string SearchTerm { get; init; }

        public ServiceSort Sorting { get; init; }

        public IEnumerable<ServiceDetailsIdServiceModel> Services { get; set; }
    }
}
