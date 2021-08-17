using Microsoft.AspNetCore.Mvc;
using Web.Services.WineAreas;

namespace Web.Controllers
{
    public class WineAreasController : Controller
    {
        private readonly IWineAreasService wineAreasService;

        public WineAreasController(IWineAreasService wineAreasService) => this.wineAreasService = wineAreasService;

        public IActionResult Map()
        {
            var wineAreas = this.wineAreasService.AllWineAreasNames();

            return View(wineAreas);
        }

        public IActionResult Details(int wineAreaId)
        {
            var areaDetails = wineAreasService.Details(wineAreaId);

            return View(areaDetails);
        }
    }
}
