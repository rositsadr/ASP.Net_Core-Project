using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructures;
using Web.Services.Orders;
using static Web.WebConstants;

namespace Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService) => this.orderService = orderService;

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
