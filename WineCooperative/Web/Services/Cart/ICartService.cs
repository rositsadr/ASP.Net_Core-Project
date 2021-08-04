using System.Collections.Generic;
using Web.Services.Cart.Models;

namespace Web.Services.Cart
{
    public interface ICartService
    {
        public IEnumerable<CartItemViewServiceModel> UsersCart(string userId);

        public bool AddProductToCart(string productId, string userId);
    }
}
