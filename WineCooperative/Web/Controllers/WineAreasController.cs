using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models.WineAreas;

namespace Web.Controllers
{
    public class WineAreasController : Controller
    {
        [Authorize]
        public IActionResult Add() => View();

        [HttpPost]
        [Authorize]
        public IActionResult Add(WineAreaAddingModel wineArea) => View(wineArea);

        public IActionResult Map() => View();

        public IActionResult Ditails() => View();
    }
}
