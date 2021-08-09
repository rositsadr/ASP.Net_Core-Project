using System.Collections.Generic;
using Web.Services.Orders.Models;

namespace Web.Services.Orders
{
    public interface IOrderService
    {
        public int CreateOrder(string userId);

        public void finalizeOrder(string userId, int orderId);

        public void RemoveOrder(int orderId);

        public IEnumerable<OrderServiceModel> UsersOrders(string userId);
    }
}
