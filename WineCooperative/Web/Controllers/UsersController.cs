using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Infrastructures;
using Web.Models.Users;
using Web.Services.Manufacturers;
using Web.Services.Products;
using Web.Services.Services;
using Web.Services.Users;
using static Web.WebConstants;

namespace Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IProductService productService;
        private readonly IServiceService serviceService;
        private readonly IManufacturerService manufacturerService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public UsersController(IProductService productService, IServiceService serviceService, IManufacturerService manufacturerService, IUserService userService, IMapper mapper)
        {
            this.productService = productService;
            this.serviceService = serviceService;
            this.manufacturerService = manufacturerService;
            this.userService = userService;
            this.mapper = mapper;
        }

        public IActionResult EditAdditionalData(string userId)
        {
            if(User.GetId() != userId || !User.IsAdmin())
            {
                return BadRequest();
            }

            var info = userService.Edit(userId);

            if(info == null)
            {
                return BadRequest();
            }

            var infoToEdit = mapper.Map<AdditionalUserInfoAddingModel>(info);

            return View(infoToEdit);
        }

        [HttpPost]
        public IActionResult EditAdditionalData(AdditionalUserInfoAddingModel info, string userId)
        {
            if (User.GetId() != userId || !User.IsAdmin())
            {
                return BadRequest();
            }

            if(!ModelState.IsValid)
            {
                return View(info);
            }

            userService.ApplyChanges(userId, info.FirstName, info.LastName, info.Address.Street, info.Address.TownName, info.Address.ZipCode, info.Address.CountryName);

            if(User.IsAdmin())
            {
                return RedirectToAction("AllMembers","Users");
            }

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

        //TODO:
        public IActionResult MyOrders()
        {
            return View();
        }

        public IActionResult BecomeMember()
        {
            if(User.IsMember() || User.IsAdmin())
            {
                return BadRequest();
            }

            return View();
        }

        public IActionResult Apply(string id)
        {
            if(User.GetId() != id)
            {
                return BadRequest();
            }

            if(User.IsMember()|| userService.UserApplyed(id))
            {
               this.TempData[ErrorMessageKey]= "You are a member or your request is pendding!";

                return RedirectToAction("Index","Home");
            }

            userService.ApplyForMember(id);

            this.TempData[SuccessMessageKey] = "You successfuly applyed for member. Now please check you additional data.";
            return RedirectToPage("/Account/Manage/Index",new { area = "Identity" });
        }
    }
}
