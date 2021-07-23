using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Data;
using Web.Infrastructures;
using Web.Models;
using Web.Models.Manufacturers;
using Web.Models.Products;
using Web.Services.Products;

namespace Web.Controllers
{
    public class ManufacturersController : Controller
    {
        private readonly WineCooperativeDbContext data;

        public ManufacturersController(WineCooperativeDbContext data) => this.data = data;

        public IActionResult All()
        {
            var members = data.Manufacturers
                .Select(m => new ManufacturerViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    PhoneNumber = m.PhoneNumber,
                    Description = m.Description,
                    Address = new ManufacturerAddressViewModel
                    {
                        Street = m.Address.Street,
                        ZipCode = m.Address.ZipCode,
                        TownName = m.Address.Town.Name
                    }
                })
                .ToList();

            return View(members);
        }

        [Authorize]
        public IActionResult Add() => View();

        [Authorize]
        [HttpPost]
        public IActionResult Add(ManufacturerAddingModel member)
        {
            if(data.Manufacturers.Any(m=>m.Name == member.Name))
            {
                this.ModelState.AddModelError(nameof(member.Name), "This member/manufacturer is already in the list.");
            }

            if (!this.ModelState.IsValid)
            {
                return View(member);
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

            var currantTown = data.Towns.Where(t => t.Name == member.Address.TownName).FirstOrDefault();

            if (currantTown == null)
            {
                currantTown = new Town { Name = member.Address.TownName, Country = country};

                data.Towns.Add(currantTown);
                data.SaveChanges();
            }

            var address = new Address
            {
                Street = member.Address.Street,
                Town = currantTown,
                ZipCode = member.Address.ZipCode                
            };

            if (!(data.Addresses.Any(a=>a.Street == member.Address.Street
                         && a.Town.Id==currantTown.Id 
                         && a.ZipCode == member.Address.ZipCode)))
            {
                data.Addresses.Add(address);
            }

            var manufacturer = new Manufacturer
            {
                Name = member.Name,
                PhoneNumber = member.PhoneNumber,
                Email = member.Email,
                Description = member.Description,
                Address = address,
                UserId = this.User.GetId(),
            };

            data.Manufacturers.Add(manufacturer);
            data.SaveChanges();

            return RedirectToAction("All");
        }

        public IActionResult Details() => View();

        public IActionResult Services(string memberId) => View();
    }
}
