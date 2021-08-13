using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructures;
using Web.Services.Cart;
using static Web.WebConstants;

namespace Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService cartService;
       
        public CartController(ICartService cartService) => this.cartService = cartService;

        public IActionResult MyCart(string userId)
        {
            if(this.UserNotAuthorize(userId))
            {
                return Unauthorized();
            }

            var usersProducts = cartService.UsersCart(userId);

            return View(usersProducts);
        }

        public IActionResult AddToCart(string productId, string userId)
        {
            if (this.UserNotAuthorize(userId))
            {
                return Unauthorized();
            }

            var success = cartService.AddProductToCart(productId, userId);

            if (success)
            {
                this.TempData[SuccessMessageKey] = string.Format(SuccesssfulyAdded, "product to your cart");
            }
            else
            {
                this.TempData[ErrorMessageKey] = "No product added to your cart!";
            }

            return RedirectToAction("All","Products");
        }

        public IActionResult Add(string productId, string userId)
        {
            if(this.UserNotAuthorize(userId))
            {
                return Unauthorized();
            }

            var success = cartService.AddFunction(productId, userId);

            if(success)
            {
                return RedirectToAction("MyCart", new { userId = userId });
            }
            else
            {
                return BadRequest("Not Existing item in your cart.");
            }
        }

        public IActionResult Remove(string productId, string userId)
        {
            if (userId != User.GetId())
            {
                return BadRequest();
            }

            cartService.RemoveFunction(productId, userId);

            return RedirectToAction("MyCart", new { userId = userId });
        }

        public IActionResult Delete(string productId, string userId)
        {
            if(this.UserNotAuthorize(userId))
            {
                return Unauthorized();
            }

            cartService.Delete(productId, userId);

            this.TempData[SuccessMessageKey] = string.Format(SuccessfullyDeleted, "product from your cart");
            return RedirectToAction("MyCart", new { userId = userId });
        }

        private bool UserNotAuthorize(string userId) => this.User
            .GetId() != userId || User.IsAdmin();
    }
}
