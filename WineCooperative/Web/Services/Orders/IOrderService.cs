namespace Web.Services.Orders
{
    public interface IOrderService
    {
        public int CreateOrder(string userId);

        public void finalizeOrder(string userId, int orderId);

        public void RemoveOrder(int orderId);
    }
}
