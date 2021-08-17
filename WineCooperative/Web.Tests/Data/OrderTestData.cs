using System.Collections.Generic;
using System.Linq;
using Web.Data.Models;

namespace Web.Tests.Data
{
    public static class OrderTestData
    {
        public static Order OrderWithId(int orderId) =>
            new Order
            {
                Id = orderId,
            };

        public static List<OrderProduct> OrderProducts(int count, int orderId) =>
            Enumerable.Range(1, count)
                .Select(i => new OrderProduct
                {
                    OrderId = orderId,
                    ProductId = i.ToString(),
                    Quantity = i
                })
                .ToList();

        public static Order OrderWithUser(int orderId, string userId,int count ) =>
            new Order
            {
                Id = orderId,
                UserId = userId,
                OrderProducts = OrderProducts(count,orderId)
            };

        public static Order OrderWithUserAndProduct(int productsCount, int orderId, string userId)
        {
            var products = Enumerable
                .Range(1, productsCount)
                .Select(i => new Product
                {
                    Id = i.ToString(),
                    Name = $"Product {i}",
                    Price = i,
                });

            var order = new Order
            {
                Id = orderId,
                UserId = userId,
                OrderProducts =new List<OrderProduct>()
            };

            foreach (var product in products)
            {
                var orderProduct = new OrderProduct
                {
                    OrderId = orderId,
                    ProductId = product.Id,
                    Product = product,
                    Quantity = 1
                };

                order.OrderProducts.Add(orderProduct);
            }

            return order;
        }
    }
}
