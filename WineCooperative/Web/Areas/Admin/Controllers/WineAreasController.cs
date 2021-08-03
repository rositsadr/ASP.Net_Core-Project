using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.WineAreas;

namespace Web.Areas.Admin.Controllers
{
    public class WineAreasController : Controller
    {
        [Authorize]
        public IActionResult Add() => View();

        [HttpPost]
        [Authorize]
        public IActionResult Add(WineAreaAddingModel wineArea) => View(wineArea);

        public IActionResult All() => View();
    }
}
