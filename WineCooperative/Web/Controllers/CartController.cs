
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Data.Models;
using Web.Infrastructures;
using Web.Services.Cart;
using Web.Services.Cart.Models;

namespace Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService cartService;
       
        public CartController(ICartService cartService) => this.cartService = cartService;

        public IActionResult MyCart(string userId)
        {
            if(this.User.GetId() != userId || User.IsAdmin())
            {
                return Unauthorized();
            }

            var usersProducts = cartService.UsersCart(userId);

            return View(usersProducts);
        }

        public IActionResult AddToCart(string productId, string userId)
        {
            if (this.User.GetId() != userId || User.IsAdmin())
            {
                return Unauthorized();
            }

            cartService.AddProductToCart(productId, userId);

            return RedirectToAction("All","Products");
        }

        public IActionResult Delete(string id) => RedirectToAction("MyCart");

        [Authorize]
        public IActionResult Add(string productId, string userId)
        {
            if(userId != User.GetId())
            {
                return BadRequest();
            }

            cartService.AddFunction(productId, userId);

            return Redirect("MyCart?userId=" + userId);
        }

        [Authorize]
        public IActionResult Remove(string productId, string userId)
        {
            if (userId != User.GetId())
            {
                return BadRequest();
            }

            cartService.RemoveFunction(productId, userId);

            return Redirect("MyCart?userId=" + userId);
        }
    }
}
