using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructures;
using Web.Models.Manufacturers;
using Web.Services.Manufacturers;
using static Web.Services.Constants;

namespace Web.Controllers
{
    public class ManufacturersController : Controller
    {
        private readonly IManufacturerService manufacturerService;
        private readonly IMapper mapper;

        public ManufacturersController(IManufacturerService manufacturerService, IMapper mapper)
        {
            this.manufacturerService = manufacturerService;
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
                return View();
            };

            return RedirectToAction("BecomeMember", "Users");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(ManufacturerModel manufacturer)
        {
            if (User.IsAdmin())
            {
                return Unauthorized();
            }

            if (User.IsMember())
            {
                if (manufacturerService.ManufacturerExistsByName(manufacturer.Name))
                {
                    this.ModelState.AddModelError(nameof(manufacturer.Name), "Manufacturer with this name already exists");
                }

                if (!this.ModelState.IsValid)
                {
                    return View(manufacturer);
                }

                manufacturerService.Create(manufacturer.Name, manufacturer.PhoneNumber, manufacturer.Email, manufacturer.Description,manufacturer.Address.Street, manufacturer.Address.ZipCode, manufacturer.Address.TownName, CountryOfManufacturing, User.GetId());

                return RedirectToAction("All");
            }

            return RedirectToAction("BecomeMember", "Users");

        }

        public IActionResult All() => View(this.manufacturerService.All());

        [Authorize]
        public IActionResult Edit(string id)
        {
            if (!(this.User.IsMember() || this.User.IsAdmin()))
            {
                return RedirectToAction("BecomeMember", "Users");
            }

            var manufacturer = this.manufacturerService.Edit(id);

            if (manufacturer == null)
            {
                return BadRequest();
            }

            if (manufacturer.UserId != User.GetId() && !this.User.IsAdmin())
            {
                return Unauthorized();
            }

            var manufacturerToEdit = mapper.Map<ManufacturerModel>(manufacturer);

            return View(manufacturerToEdit);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(ManufacturerModel manufacturer, string id)
        {
            if (!(this.User.IsMember() || this.User.IsAdmin()))
            {
                return RedirectToAction("BecomeMember", "Users");
            }

            if(!manufacturerService.ManufacturerExistsById(id))
            {
                ModelState.AddModelError(string.Empty, "The manufacturer does not exists.");
            }

            if(manufacturerService.ManufacturerExistsByName(manufacturer.Name))
            {
                ModelState.AddModelError(string.Empty, "This manufacturer already exists!");
            }

            if (!ModelState.IsValid)
            {
                return View(manufacturer);
            }

            if (!(this.manufacturerService.IsItUsersManufacturer(User.GetId(), id) || User.IsAdmin()))
            {
                return BadRequest();
            }

            this.manufacturerService.ApplyChanges(id, manufacturer.Name, manufacturer.Description, manufacturer.PhoneNumber, manufacturer.Email, manufacturer.Address.Street,manufacturer.Address.TownName, manufacturer.Address.ZipCode);

            return RedirectToAction("All");
        }
    }
}
