using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Data;
using Web.Data.Models;
using Web.Models;
using Web.Models.Services;

namespace Web.Controllers
{
    public class ServicesController : Controller
    {
        private readonly WineCooperativeDbContext data;
        private readonly UserManager<User> _userManager;

        public ServicesController(WineCooperativeDbContext data, UserManager<User> userManager)
        {
            this.data = data;
            _userManager = userManager;
        }

        public IActionResult Add() => View();

        [HttpPost]
        public IActionResult Add(ServiceAddingModel service)
        {
            if (!ModelState.IsValid)
            {
                return View(service);
            }

            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            var manifacturerId = data.UserAdditionalInformation
                .Where(a => a.Id == user.UserDataId)
                .Select(a => a.ManufacturerId)
                .FirstOrDefault();

            var serviceToAdd = new Service
            {
                Name = service.Name,
                Price = service.Price,
                ImageUrl = service.ImageUrl,
                Description = service.Description,
                ManufacturerId = manifacturerId
            };

            data.Services.Add(serviceToAdd);
            data.SaveChanges();

            return RedirectToAction("All", "Services");
        }

        public IActionResult All() => View();

        public IActionResult ServiceDetails(string Id) => View();
    }
}
