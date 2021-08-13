using System.Collections.Generic;
using System.Linq;
using Web.Data.Models;
using Web.Models;

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
                    ProductId = i.ToString()
                })
                .ToList();

        public static Order OrderWithUser(int orderId, string userId,int count ) =>
            new Order
            {
                Id = orderId,
                UserId = userId,
                OrderProducts = OrderProducts(count,orderId)
            };
    }
}
