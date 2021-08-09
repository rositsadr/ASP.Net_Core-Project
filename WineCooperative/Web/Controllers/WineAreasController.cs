using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class WineAreasController : Controller
    {
        public IActionResult Map() => View();

        public IActionResult Details() => View();
    }
}
