using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;
using Web.Data;
using Web.Data.Models;
using Web.Services.Cart.Models;

namespace Web.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly WineCooperativeDbContext data;
        private readonly IConfigurationProvider config;

        public CartService(WineCooperativeDbContext data, IMapper mapper)
        {
            this.data = data;
            this.config = mapper.ConfigurationProvider;
        }

        public IEnumerable<CartItemViewServiceModel> UsersCart(string userId) => data.ShoppingCartItems
            .Where(sci => sci.UserId == userId)
            .ProjectTo<CartItemViewServiceModel>(config)
            .ToList();

        public bool AddProductToCart(string productId, string userId)
        {
            var user = data.Users
                .Where(u => u.Id == userId)
                .FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            var product = data.Products
                .Where(p => p.Id == productId)
                .FirstOrDefault();
            
            if(product == null)
            {
                return false;
            }

            if(data.ShoppingCartItems.Any(c=>c.ProductId == productId && c.UserId == userId))
            {
                var cartItem = data.ShoppingCartItems
                    .Where(c => c.ProductId == productId && c.UserId == userId)
                    .FirstOrDefault();

                cartItem.Quantity++;

                data.SaveChanges();
            }
            else
            {
                var cartItem = new CartItem()
                {
                    UserId = userId,
                    Quantity = 1,
                    ProductId = productId
                };

                data.ShoppingCartItems.Add(cartItem);
                data.SaveChanges();
            }

            return true;
        }
    }
}
