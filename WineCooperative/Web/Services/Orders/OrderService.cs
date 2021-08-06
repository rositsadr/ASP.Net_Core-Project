using System;
using System.Linq;
using Web.Data;
using Web.Models;
using Web.Data.Models;

namespace Web.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly WineCooperativeDbContext data;

        public OrderService(WineCooperativeDbContext data) => this.data = data;

        public int CreateOrder(string userId)
        {
            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                UserId = userId
            };

            data.Orders.Add(order);
            data.SaveChanges();

            return order.Id;          
        }

        public void finalizeOrder(string userId, int orderId)
        {
            var productsInCart = data.ShoppingCartItems
                .Where(s => s.UserId == userId)
                .ToList();

            foreach (var item in productsInCart)
            {
                var productId = item.ProductId;
                var quantity = item.Quantity;

                data.OrdersProducts.Add(new OrderProduct
                { 
                    OrderId = orderId, 
                    ProductId = productId,
                    Quantity = quantity,
                });
                
                var itemCart = data.ShoppingCartItems
                    .Where(p => p.UserId == userId && p.ProductId == productId)
                    .FirstOrDefault();

                data.ShoppingCartItems.Remove(itemCart);
                data.SaveChanges();
            }
        }

        public void RemoveOrder(int orderId)
        {
            var order = data.Orders
                .Where(o => o.Id == orderId)
                .FirstOrDefault();

            var orderProducts = data.OrdersProducts
                .Where(op => op.OrderId == orderId)
                .ToList();

            data.OrdersProducts.RemoveRange(orderProducts);
            data.Orders.Remove(order);
            data.SaveChanges();
        }
    }
}
