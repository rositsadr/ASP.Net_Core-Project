using System.Collections.Generic;
using Web.Services.Orders.Models;

namespace Web.Services.Orders
{
    public interface IOrderService
    {
        public int CreateOrderInTheDatabase(string userId);

        public bool finalizeOrder(string userId, int orderId);

        public OrderServiceModel OrderDetailsFromCart(string userId);

        public void RemoveOrder(int orderId);

        public IEnumerable<OrderServiceModel> UsersOrders(string userId);

        public bool OrderExists(int orderId, string userId);
    }
}
