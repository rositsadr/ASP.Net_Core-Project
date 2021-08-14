using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using Web.Infrastructures;
using Web.Services.Manufacturers;
using Web.Services.Orders;
using Web.Services.Products;
using Web.Services.Services;
using Web.Services.Users;
using Web.Services.Users.Models;
using static Web.WebConstants;
using static Web.Areas.AreaConstants;


namespace Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly IProductService productService;
        private readonly IServiceService serviceService;
        private readonly IManufacturerService manufacturerService;
        private readonly IOrderService orderService;
        private readonly IMemoryCache cache;

        public UsersController(IUserService userService, IProductService productService, IServiceService serviceService, IManufacturerService manufacturerService, IOrderService orderService, IMemoryCache cache)
        {
            this.userService = userService;
            this.productService = productService;
            this.serviceService = serviceService;
            this.manufacturerService = manufacturerService;
            this.orderService = orderService;
            this.cache = cache;
        }

        public IActionResult MyProducts()
        {
            if (User.IsMember())
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
            if (User.IsMember())
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
        public IActionResult MyOrders(string id)
        {
            if (User.GetId() != id)
            {
                return BadRequest();
            }

            var orders = orderService.UsersOrders(id);

            return View(orders);
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
               this.TempData[ErrorMessageKey]= "Your request is pending!";

                return RedirectToAction("Index","Home");
            }

            userService.ApplyForMember(id);

            cache.Set<List<UserInfoServiceModel>>(applyedCacheKey, null);

            this.TempData[SuccessMessageKey] = "You successfuly applyed for member. Now please check you additional data.";
            return RedirectToPage("/Account/Manage/Index",new { area = "Identity" });
        }
    }
}
