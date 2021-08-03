using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Data;
using Web.Infrastructures;
using Web.Models;
using Web.Models.Manufacturers;
using Web.Services.Addresses;
using Web.Services.Manufacturers;
using Web.Services.Products;
using static Web.Services.Constants;

namespace Web.Controllers
{
    public class ManufacturersController : Controller
    {
        private readonly IManufacturerService manufacturerService;

        public ManufacturersController(IManufacturerService manufacturerService) => this.manufacturerService = manufacturerService;

        [Authorize]
        public IActionResult Add()
        {
            if (User.IsAdmin())
            {
                return Unauthorized();
            }

            if (User.IsMember())
            {
                return View();
            };

            return RedirectToAction("BecomeMember", "Users");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(ManufacturerAddingModel manufacturer)
        {
            if (User.IsAdmin())
            {
                return Unauthorized();
            }

            if (User.IsMember())
            {
                if (manufacturerService.ManufacturerExistsByName(manufacturer.Name))
                {
                    this.ModelState.AddModelError(nameof(manufacturer.Name), "Manufacturer with this name already exists");
                }

                if (!this.ModelState.IsValid)
                {
                    return View(manufacturer);
                }

                manufacturerService.Create(manufacturer.Name, manufacturer.PhoneNumber, manufacturer.Email, manufacturer.Description,manufacturer.Address.Street, manufacturer.Address.ZipCode, manufacturer.Address.TownName, CountryOfManufacturing, User.GetId());

                return RedirectToAction("All");
            }

            return RedirectToAction("BecomeMember", "Users");

        }

        public IActionResult All() => View(this.manufacturerService.All());

        public IActionResult Edit() => View();
    }
}
