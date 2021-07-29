using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Data;
using Web.Infrastructures;
using Web.Models;
using Web.Models.Manufacturers;
using Web.Services.Manufacturers;

namespace Web.Controllers
{
    public class ManufacturersController : Controller
    {
        private readonly WineCooperativeDbContext data;
        private readonly IManufacturerService manufacturerService;

        public ManufacturersController(WineCooperativeDbContext data, IManufacturerService manufacturerService)
        {
            this.data = data;
            this.manufacturerService = manufacturerService;
        }

        public IActionResult All() => View(this.manufacturerService.All());

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
            if(data.Manufacturers.Any(m=>m.Name == manufacturer.Name))
            {
                this.ModelState.AddModelError(nameof(manufacturer.Name), "This member/manufacturer is already in the list.");
            }

            if (!this.ModelState.IsValid)
            {
                return View(manufacturer);
            }

            var country = data.Countries
                .Where(c => c.CountryName == "Bulgaria")
                .FirstOrDefault();

            if(country == null)
            {
                country = new Country
                {
                   CountryName = "Bulgaria",
                };

                data.Countries.Add(country);
                data.SaveChanges();
            }

            var currantTown = data.Towns.Where(t => t.Name == manufacturer.Address.TownName).FirstOrDefault();

            if (currantTown == null)
            {
                currantTown = new Town { Name = manufacturer.Address.TownName, Country = country};

                data.Towns.Add(currantTown);
                data.SaveChanges();
            }

            var address = new Address
            {
                Street = manufacturer.Address.Street,
                Town = currantTown,
                ZipCode = manufacturer.Address.ZipCode                
            };

            if (!(data.Addresses.Any(a=>a.Street == manufacturer.Address.Street
                         && a.Town.Id==currantTown.Id 
                         && a.ZipCode == manufacturer.Address.ZipCode)))
            {
                data.Addresses.Add(address);
            }

            var manufacturerToAdd = new Manufacturer
            {
                Name = manufacturer.Name,
                PhoneNumber = manufacturer.PhoneNumber,
                Email = manufacturer.Email,
                Description = manufacturer.Description,
                Address = address,
                UserId = this.User.GetId(),
            };

            data.Manufacturers.Add(manufacturerToAdd);
            data.SaveChanges();

            return RedirectToAction("All");
        }

        public IActionResult Details() => View();

        public IActionResult Services(string memberId) => View();
    }
}
