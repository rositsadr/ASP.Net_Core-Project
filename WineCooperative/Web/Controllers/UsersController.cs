using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Data;
using Web.Infrastructures;
using Web.Models;
using Web.Models.Users;
using Web.Services.Products;
using Web.Services.Services;

namespace Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly WineCooperativeDbContext data;
        private readonly IProductService productService;
        private readonly IServiceService serviceService;

        public UsersController(WineCooperativeDbContext data, IProductService productService, IServiceService serviceService)
        {
            this.data = data;
            this.productService = productService;
            this.serviceService = serviceService;
        }

        [Authorize]
        public IActionResult AdditionalUserInfo() => View();

        [HttpPost]
        [Authorize]
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
            var userId = this.User.GetId();

            var products = productService.ProductsByUser(userId);

            return View(products);
        }

        public IActionResult MyServices()
        {
            var userId = this.User.GetId();

            var services = serviceService.ServicesByUser(userId); 

            return View(services);
        }

        public IActionResult MyManufecturers() => View();

        public IActionResult BecomeMember(string id) => View();
    }
}
