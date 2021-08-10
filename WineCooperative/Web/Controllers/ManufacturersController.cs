using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructures;
using Web.Models.Manufacturers;
using Web.Services.Manufacturers;
using static Web.Services.Constants;
using static Web.WebConstants;

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

            this.TempData[ErrorMessageKey] = NotPermitted;
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

                this.TempData[SuccessMessageKey] = string.Format(SuccesssfulyAdded,"manufacturer");
                return RedirectToAction("All");
            }

            this.TempData[ErrorMessageKey] = NotPermitted;
            return RedirectToAction("BecomeMember", "Users");

        }

        public IActionResult All() => View(this.manufacturerService.All());

        [Authorize]
        public IActionResult Edit(string id)
        {
            if (!(this.User.IsMember() || this.User.IsAdmin()))
            {
                this.TempData[ErrorMessageKey] = NotPermitted;
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
                this.TempData[ErrorMessageKey] = NotPermitted;
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

            if (!(this.manufacturerService.IsUsersManufacturer(User.GetId(), id) || User.IsAdmin()))
            {
                return BadRequest();
            }

            this.manufacturerService.ApplyChanges(id, manufacturer.Name, manufacturer.Description, manufacturer.PhoneNumber, manufacturer.Email, manufacturer.Address.Street,manufacturer.Address.TownName, manufacturer.Address.ZipCode);

            this.TempData[SuccessMessageKey] = string.Format(SuccesssfulyEdited,"manufacturer");
            return RedirectToAction("All");
        }

        public IActionResult Details(string id)
        {
            var manufacturer = manufacturerService.Details(id);

            if (manufacturer == null)
            {
                this.TempData[ErrorMessageKey] = "The manufacturer you are trying to view is not in the list!";
                return RedirectToAction("All");
            }

            return View(manufacturer);
        }

        [Authorize]
        public IActionResult Delete(string id)
        {
            var userId = User.GetId();

            if (!(this.User.IsMember() || this.User.IsAdmin()))
            {
                this.TempData[ErrorMessageKey] = NotPermitted;
                return RedirectToAction("BecomeMember", "Users");
            }

            if (!this.manufacturerService.IsUsersManufacturer(userId, id))
            {
                return Unauthorized();
            }

            var success = this.manufacturerService.Delete(id);

            if (success)
            {
                this.TempData[SuccessMessageKey] = string.Format(SuccessfullyDeleted, "manufacturer");
            }
            else
            {
                this.TempData[ErrorMessageKey] = string.Format(NotExistToDelete, "Manufacturer");
            }

            return RedirectToAction("All");
        }
    }
}
