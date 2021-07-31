using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Infrastructures;
using Web.Models;
using Web.Models.Services;
using Web.Services.Manufacturers;
using Web.Services.Services;

namespace Web.Controllers
{
    public class ServicesController : Controller
    {
        private readonly WineCooperativeDbContext data;
        private readonly IManufacturerService manufacturerService;
        private readonly IServiceService serviceService;


        public ServicesController(WineCooperativeDbContext data, IManufacturerService manufacturerService, IServiceService serviceService)
        {
            this.data = data;
            this.manufacturerService = manufacturerService;
            this.serviceService = serviceService;
        }

        [Authorize]
        public IActionResult Add()
        {
            if (User.IsAdmin())
            {
                return Unauthorized();
            }

            if (User.IsMember())
            {
                return View(new ServiceAddingModel
                {
                    Manufacturers = this.manufacturerService.ManufacturersByUser(User.GetId())
                });
            }

            return RedirectToAction("BecomeMember", "Users");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(ServiceAddingModel service)
        {
            if (User.IsAdmin())
            {
                return Unauthorized();
            }

            if (User.IsMember())
            {
                if (!manufacturerService.ManufacturerExistsById(service.ManufacturerId))
                {
                    this.ModelState.AddModelError(string.Empty, "The Manufacturer does not exists.");
                }

                if(serviceService.ServiceExists(User.GetId(),service.Name))
                {
                    this.ModelState.AddModelError(string.Empty, "The service already exists. Check your Services.");
                }

                if (!ModelState.IsValid)
                {
                    service.Manufacturers = manufacturerService.ManufacturersByUser(User.GetId());

                    return View(service);
                }

                serviceService.Create(service.Name, service.Price, service.ImageUrl, service.Description, service.ManufacturerId);

                return RedirectToAction("All", "Services");
            }

            return RedirectToAction("BecomeMember", "Users");
        }

        public IActionResult All() => View();

        public IActionResult Details(string Id) => View();
    }
}
