using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;
using Web.Models.Services;

namespace Web.Controllers
{
    public class ServicesController : Controller
    {
        private readonly WineCooperativeDbContext data;

        public ServicesController(WineCooperativeDbContext data)
        {
            this.data = data;
        }

        [Authorize]
        public IActionResult Add() => View();

        [HttpPost]
        [Authorize]
        public IActionResult Add(ServiceAddingModel service)
        {
            if (!ModelState.IsValid)
            {
                return View(service);
            }

            var serviceToAdd = new Service
            {
                Name = service.Name,
                Price = service.Price,
                ImageUrl = service.ImageUrl,
                Description = service.Description,
            };

            data.Services.Add(serviceToAdd);
            data.SaveChanges();

            return RedirectToAction("All", "Services");
        }

        public IActionResult All() => View();

        public IActionResult Details(string Id) => View();
    }
}
