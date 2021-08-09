using Microsoft.AspNetCore.Mvc;
using Web.Infrastructures;
using Web.Services.Orders;
using static Web.WebConstants;

namespace Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public IActionResult MyOrders()
        {
            var orders = orderService.UsersOrders(User.GetId());

            return View(orders);
        }

        //TODO:
        public IActionResult Delete(string id)
        {
            return null;
        }
    }
}
