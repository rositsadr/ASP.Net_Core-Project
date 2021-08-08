﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Infrastructures;
using Web.Models.Services;
using Web.Services.Manufacturers;
using Web.Services.Services;
using static Web.WebConstants;

namespace Web.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IManufacturerService manufacturerService;
        private readonly IServiceService serviceService;
        private readonly IMapper mapper;


        public ServicesController(IManufacturerService manufacturerService, IServiceService serviceService, IMapper mapper)
        {
            this.manufacturerService = manufacturerService;
            this.serviceService = serviceService;
            this.mapper = mapper;
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
                return View(new ServiceModel
                {
                    Manufacturers = this.manufacturerService.ManufacturersNameByUser(User.GetId())
                });
            }

            return RedirectToAction("BecomeMember", "Users");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(ServiceModel service)
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

                if(serviceService.ServiceExists(service.ManufacturerId,service.Name))
                {
                    this.ModelState.AddModelError(string.Empty, "The service already exists. Check your Services.");
                }

                if (!ModelState.IsValid)
                {
                    service.Manufacturers = manufacturerService.ManufacturersNameByUser(User.GetId());

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

            if(!User.IsAdmin())
            {
                servicesResult.Services = servicesResult.Services
                    .Where(s => s.Available);
            }
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

        [Authorize]
        public IActionResult Edit(string id)
        {
            var userId = this.User.GetId();

            if (!(this.User.IsMember() || this.User.IsAdmin()))
            {
                return RedirectToAction("BecomeMember", "Users");
            }

            var servicesToEdit = serviceService.Edit(id);

            if (servicesToEdit == null)
            {
                return BadRequest();
            }

            if (servicesToEdit.UserId != userId && !this.User.IsInRole(AdministratorRole))
            {
                return Unauthorized();
            }

            var service = mapper
                .Map<ServiceModel>(servicesToEdit);

            var manufacturers = this.manufacturerService.AllManufacturers();

            if (User.IsMember())
            {
                manufacturers = this.manufacturerService.ManufacturersNameByUser(userId);
            }

            service.Manufacturers = manufacturers;

            return View(service);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(ServiceModel service, string id)
        {
            var userId = User.GetId();

            if (!(this.User.IsMember() || User.IsAdmin()))
            {
                return RedirectToAction("BecomeMember", "Users");
            }

            if (!manufacturerService.ManufacturerExistsById(service.ManufacturerId))
            {
                this.ModelState.AddModelError(nameof(service.ManufacturerId), "The Manufacturer does not exists.");
            }

            if (serviceService.ServiceExists(service.ManufacturerId, service.Name))
            {
                this.ModelState.AddModelError(string.Empty, "This service is already in the list.");
            }


            if (!ModelState.IsValid)
            {
                service.Manufacturers = this.manufacturerService.ManufacturersNameByUser(userId);

                return View(service);
            }

            if (!(this.serviceService.IsItUsersService(userId, id) || User.IsAdmin()))
            {
                return BadRequest();
            }

            serviceService.ApplyChanges(id, service.Name, service.Description, service.ImageUrl, service.Price, service.Available, service.ManufacturerId);

            return RedirectToAction("All");
        }

        public IActionResult Details(string id)
        {
            var service = serviceService.Details(id);

            if(service == null)
            {
                RedirectToAction("All");
            }

            return View(service);
        }

        public IActionResult Delete(string id)
        {
            var userId = User.GetId();

            if (!(this.User.IsMember() || this.User.IsAdmin()))
            {
                return RedirectToAction("BecomeMember", "Users");
            }

            if (!(this.serviceService.IsItUsersService(userId, id) || User.IsAdmin()))
            {
                return Unauthorized();
            }

            serviceService.Delete(id);

            return RedirectToAction("All");
        }
    }
}
