using Microsoft.AspNetCore.Mvc;
using Web.Models.WineAreas;

namespace Web.Controllers
{
    public class WineAreasController : Controller
    {
        public IActionResult Add() => View();

        [HttpPost]
        public IActionResult Add(WineAreaAddingModel wineArea) => View(wineArea);

        public IActionResult Map() => View();

        public IActionResult Ditails() => View();
    }
}
