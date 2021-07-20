
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        public IActionResult MyCart()
        {

            return View();
        }
        public IActionResult AddToCart(string wineId) => RedirectToAction("MyCart");

        public IActionResult Delete(string wineId) => RedirectToAction("MyCart");
    }
}
