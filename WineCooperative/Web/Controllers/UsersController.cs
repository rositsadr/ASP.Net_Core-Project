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

        public IActionResult AdditionalUserInfo()
        {
            if (User.IsAdmin())
            {
                return Unauthorized();
            }

            return View();
        }

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

        public IActionResult EditAdditionalInfo(string userId)
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

        //[HttpPost]
        //public IActionResult EditAdditionalInfo(AdditionalUserInfoAddingModel info, string userId)
        //{
        //    if(User.GetId() != userId || !User.IsAdmin())
        //    {
        //        return BadRequest();
        //    }


        //}

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
                return BadRequest();
            }

            userService.Apply(id);

            if (userService.UserHasAdditionaInfo(id))
            {
                return RedirectToAction("EditAdditionalInfo");
            }

            return RedirectToAction("AdditionalUserInfo");
        }
    }
}
