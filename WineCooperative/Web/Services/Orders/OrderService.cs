using System;
using System.Linq;
using Web.Data;
using Web.Models;
using Web.Data.Models;
using System.Collections.Generic;
using Web.Services.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace Web.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly WineCooperativeDbContext data;

        public OrderService(WineCooperativeDbContext data) => this.data = data;

        public int CreateOrderInTheDatabase(string userId)
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

        public bool finalizeOrder(string userId, int orderId)
        {
            var productsInCart = data.ShoppingCartItems
                .Where(s => s.UserId == userId)
                .ToList();

            if (productsInCart == null)
            {
                return false;
            }

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

                data.ShoppingCartItems.Remove(item);
                data.SaveChanges();
            }

            return true;
        }

        public OrderServiceModel OrderDetailsFromCart(string userId)
        {
            var newOrderItems = data.ShoppingCartItems
                .Where(sh => sh.UserId == userId)
                .Include(i=>i.Product)
                .ToList();

            if (newOrderItems == null)
            {
                return null;
            }

            var newOrder = new OrderServiceModel()
            {
                Products = new List<OrderProductServiceModel>()
            };

            foreach (var item in newOrderItems)
            {
                var newItem = new OrderProductServiceModel
                {
                     ProductName=item.Product.Name,
                      Price = item.Product.Price,
                      Quantity = item.Quantity,
                };

                newItem.TotalPrice = newItem.Price * newItem.Quantity;

                newOrder.Products.Add(newItem);
            }

            newOrder.TotalAmount = newOrder.Products.Sum(p => p.TotalPrice);
            newOrder.Products = newOrder.Products.OrderBy(p => p.ProductName).ToList();

            return newOrder;
        }

        public bool OrderExists(int orderId, string userId)
        {
            return data.Orders.Any(o => o.Id == orderId && o.UserId == userId);
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

        public IEnumerable<OrderServiceModel> UsersOrders(string userId)
        {
            var ordersProducts = data.OrdersProducts
                .Where(o => o.Order.UserId == userId)
                .Include(o=>o.Product)
                .AsQueryable();

            if(ordersProducts == null)
            {
                return null;
            }

            var ordersId = ordersProducts
                 .Select(o => o.OrderId)
                 .Distinct()
                 .ToList();

            var orders = new List<OrderServiceModel>();

            foreach (var id in ordersId)
            {
                var order = new OrderServiceModel
                {
                    OrderId = id,
                    Products = new List<OrderProductServiceModel>()
                };

                foreach (var item in ordersProducts)
                {
                    if(item.OrderId == id)
                    {
                        var product = new OrderProductServiceModel
                        {
                            ProductName = item.Product.Name,
                            Price = item.Product.Price,
                            Quantity = item.Quantity,
                            TotalPrice = item.Product.Price * item.Quantity
                        };

                        order.Products.Add(product);
                    }
                }

                order.TotalAmount = order.Products.Sum(p => p.TotalPrice);

                orders.Add(order);
            }

            return orders;
        }
    }
}
