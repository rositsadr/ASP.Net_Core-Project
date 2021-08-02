using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Infrastructures;
using Web.Models.Services;
using Web.Services.Manufacturers;
using Web.Services.Services;

namespace Web.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IManufacturerService manufacturerService;
        private readonly IServiceService serviceService;


        public ServicesController(IManufacturerService manufacturerService, IServiceService serviceService)
        {
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

                serviceService.Create(service.Name, service.Price, service.ImageUrl, service.Description, service.ManufacturerId, service.Available);

                return RedirectToAction("All", "Services");
            }

            return RedirectToAction("BecomeMember", "Users");
        }

        public IActionResult All([FromQuery] ServiceSearchPageModel query, string id = null)
        {
            var servicesResult = this.serviceService.All(ServiceSearchPageModel.servicesPerPage, query.CurrantPage, query.SearchTerm, query.Sorting);

            if (id != null)
            {
                servicesResult.Services = servicesResult.Services
                    .Where(s => s.ManufacturerId == id)
                    .ToList();
            }

            query.TotalServices = servicesResult.TotalServices;
            query.Services = servicesResult.Services;

            return View(query);
        }
    }
}
