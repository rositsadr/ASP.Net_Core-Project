
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        public IActionResult MyCart(string id) => View();

        public IActionResult AddToCart(string id) => RedirectToAction("MyCart");

        public IActionResult Delete(string id) => RedirectToAction("MyCart");
    }
}
