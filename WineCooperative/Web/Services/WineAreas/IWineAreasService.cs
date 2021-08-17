using System.Collections.Generic;
using Web.Services.WineAreas.Models;

namespace Web.Services.WineAreas
{
    public interface IWineAreasService
    {
        public List<WineAreaDisplayServiceModel> AllWineAreasNames();

        public WineAreaServiceModel Details(int wineAreaId);
    }
}
