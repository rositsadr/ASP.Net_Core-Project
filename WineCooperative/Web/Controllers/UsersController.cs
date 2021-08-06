using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Data;
using Web.Infrastructures;
using Web.Models;
using Web.Models.Users;
using Web.Services.Addresses;
using Web.Services.Manufacturers;
using Web.Services.Products;
using Web.Services.Services;
using Web.Services.Users;

namespace Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly WineCooperativeDbContext data;
        private readonly IProductService productService;
        private readonly IServiceService serviceService;
        private readonly IManufacturerService manufacturerService;
        private readonly IUserService userService;


        public UsersController(WineCooperativeDbContext data, IProductService productService, IServiceService serviceService, IManufacturerService manufacturerService, IUserService userService)
        {
            this.data = data;
            this.productService = productService;
            this.serviceService = serviceService;
            this.manufacturerService = manufacturerService;
            this.userService = userService;
        }

        public IActionResult AdditionalUserInfo() => View();

        [HttpPost]
        public IActionResult AdditionalUserInfo(AdditionalUserInfoAddingModel userInfo)
        {
            if(!this.ModelState.IsValid)
            {
                return View(userInfo);
            }

            this.userService.AddUserAdditionalInfo(User.GetId(),userInfo.FirstName, userInfo.LastName, userInfo.Address.Street, userInfo.Address.TownName, userInfo.Address.ZipCode, userInfo.Address.CountryName);

            return RedirectToAction("Index","Home");
        }

        public IActionResult MyProducts()
        {
            if(User.IsMember())
            {
                var products = productService.ProductsByUser(User.GetId());

                return View(products);
            }

            if (User.IsAdmin())
            {
                return Unauthorized();
            }

            return RedirectToAction("BecomeMember");
        }

        public IActionResult MyServices()
        {
            if(User.IsMember())
            {
                var services = serviceService.ServicesByUser(User.GetId());

                return View(services);
            }

            if (User.IsAdmin())
            {
                return Unauthorized();
            }

            return RedirectToAction("BecomeMember");

        }

        public IActionResult MyManufacturers()
        {
            if (User.IsMember())
            {
                var manufacturers = manufacturerService.ManufacturersByUser(User.GetId());

                return View(manufacturers);
            }

            if (User.IsAdmin())
            {
                return Unauthorized();
            }

            return RedirectToAction("BecomeMember");
        }

        public IActionResult MyOrders()
        {
            //TODO:
            return View();
        }

        public IActionResult BecomeMember() => View();


    }
}
