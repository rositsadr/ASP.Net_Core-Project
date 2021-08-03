using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Areas.Admin.Controllers
{
    public class GrapeVarietiesController : Controller
    {
        public IActionResult Add() => View();

        public IActionResult All() => View();
    }
}
