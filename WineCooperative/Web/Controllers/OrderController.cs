using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructures;
using Web.Services.Orders;
using Web.Services.Users;
using static Web.WebConstants;

namespace Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IUserService userService;

        public OrderController(IOrderService orderService, IUserService userService)
        {
            this.orderService = orderService;
            this.userService = userService;
        }

        public IActionResult OrderDetails(string id)
        {
            var details = orderService.OrderDetailsFromCart(id);

            if(details == null)
            {
                this.TempData[ErrorMessageKey] = "Your cart is empty!";
                return RedirectToAction("All", "Products");
            }

            return View(details);
        }

        public IActionResult FinalizeOrder(string id)
        {
            if (User.GetId() != id)
            {
                return BadRequest();
            }

            if(!userService.UserHasAdditionaInfo(id))
            {
                this.TempData[ErrorMessageKey] = "Please fill up your data before order.";
                return RedirectToPage("/Account/Manage/Index", new { area = "Identity" });
            }

            int orderId = orderService.CreateOrderInTheDatabase(id);

            var success = orderService.finalizeOrder(id, orderId);

            if(success)
            {
                this.TempData[SuccessMessageKey] = "Your order is done successfuly!";
                return RedirectToAction("MyOrders", "Users", new { id = id });
            }

            this.TempData[ErrorMessageKey] = "No products in you Cart!";
            return RedirectToAction("All", "Products");
        }

        public IActionResult DeleteFromArchives(int id)
        {
            var userId = User.GetId();

           if (orderService.OrderExists(id, userId))
            {
                orderService.RemoveOrder(id);

                this.TempData[SuccessMessageKey] = string.Format(SuccessfullyDeleted, "order from your archives");
                return RedirectToAction("MyOrders", "Users", new { id = userId });
            }

            this.TempData[ErrorMessageKey] = "This order is not in the list!";
            return RedirectToAction("MyOrders", "Users", new { id = userId });
        }
    }
}
