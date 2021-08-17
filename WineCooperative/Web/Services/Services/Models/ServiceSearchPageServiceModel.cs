using System.Collections.Generic;
using Web.ViewModels.Services.Enums;

namespace Web.Services.Services.Models
{
    public class ServiceSearchPageServiceModel
    {
        public int ServicesPerPage { get; init; }

        public int CurrantPage { get; init; }

        public int TotalServices { get; set; }

        public string SearchTerm { get; init; }

        public ServiceSort Sorting { get; init; }

        public IEnumerable<ServiceDetailsIdServiceModel> Services { get; set;}
    }
}
