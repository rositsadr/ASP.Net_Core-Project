using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructures;
using Web.Services.Manufacturers;
using Web.Services.Orders;
using Web.Services.Products;
using Web.Services.Services;
using Web.Services.Users;
using static Web.WebConstants;

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

        public UsersController(IUserService userService, IProductService productService, IServiceService serviceService, IManufacturerService manufacturerService, IOrderService orderService)
        {
            this.userService = userService;
            this.productService = productService;
            this.serviceService = serviceService;
            this.manufacturerService = manufacturerService;
            this.orderService = orderService;
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
               this.TempData[ErrorMessageKey]= "You are a member or your request is pendding!";

                return RedirectToAction("Index","Home");
            }

            userService.ApplyForMember(id);

            this.TempData[SuccessMessageKey] = "You successfuly applyed for member. Now please check you additional data.";
            return RedirectToPage("/Account/Manage/Index",new { area = "Identity" });
        }
    }
}
