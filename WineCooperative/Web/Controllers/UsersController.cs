using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Data;
using Web.Data.Models;
using Web.Infrastructures;
using Web.Models;
using Web.Models.Users;

namespace Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly WineCooperativeDbContext data;

        public UsersController(WineCooperativeDbContext data)
        {
            this.data = data;
        }

        public IActionResult AdditionalUserInfo() => View();

        [HttpPost]
        public IActionResult AdditionalUserInfo(AdditionalUserInfoAddingModel userInfo)
        {
            if(!this.ModelState.IsValid)
            {
                return View(userInfo);
            }

            var country = data.Countries
                .Where(c => c.CountryName == userInfo.Address.CountryName)
                .FirstOrDefault();

            if(country == null)
            {
                country = new Country
                {
                    CountryName = userInfo.Address.CountryName
                };

                data.Countries.Add(country);
                data.SaveChanges();
            }

            var town = data.Towns
                .Where(t => t.Name == userInfo.Address.TownName)
                .FirstOrDefault();

            if(town == null)
            {
                town = new Town
                {
                    Name = userInfo.Address.TownName,
                    Country = country
                };
            }

            var address = new Address
            {
                Street = userInfo.Address.Street,
                ZipCode = userInfo.Address.ZipCode,
                Town = town,                 
            };

            var userInformation = new UserAdditionalInformation
            {
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Address = address,
            };

            return RedirectToAction("Index","Home");
        }

        public IActionResult MyProducts()
        {
            var products = data.Products
                .Where(p => p.Manufacturer.UserId == this.User.GetId())
                .Select(p => new UserProductsViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description,
                    Price = p.Price,
                    Color = p.Color.Name,
                    Taste = p.Taste.Name,
                    ManufactureYear = p.ManufactureYear,
                    Manufacturer = p.Manufacturer.Name,
                    InStock = p.InStock,
                    WineArea = p.WineArea.Name,
                })
                .ToList();

            return View(products);
        }

        public IActionResult MyServices()
        {
            return View();
        }
    }
}
