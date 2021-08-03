using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models.WineAreas;

namespace Web.Controllers
{
    public class WineAreasController : Controller
    {
        public IActionResult Map() => View();

        public IActionResult Details() => View();
    }
}
