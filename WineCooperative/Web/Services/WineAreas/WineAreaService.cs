using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Data.Models;
using Web.Services.Manufacturers.Models;
using Web.Services.WineAreas.Models;

namespace Web.Services.WineAreas
{
    public class WineAreaService : IWineAreasService
    {
        private readonly WineCooperativeDbContext data;
        private readonly IConfigurationProvider config;

        public WineAreaService(WineCooperativeDbContext data, IMapper mapper)
        {
            config = mapper.ConfigurationProvider;
            this.data = data;
        }

        public List<WineAreaDisplayServiceModel> AllWineAreasNames() => data.WineAreas
                .ProjectTo<WineAreaDisplayServiceModel>(config)
                .ToList();

        public WineAreaServiceModel Details(int wineAreaId)
        {
            var wineArea = data.WineAreas
                .Where(wa=>wa.Id == wineAreaId)
                .ProjectTo<WineAreaServiceModel>(config)
                .FirstOrDefault();

            var manufacturers = data.Manufacturers
                .Where(m => m.Products.Any(p => p.WineAreaId == wineAreaId) && m.IsFunctional)
                .Distinct()
                .ProjectTo<ManufacturerNameServiceModel>(config)
                .ToList();

            wineArea.Manufacturers = manufacturers;

            return wineArea;
        }
    }
}
