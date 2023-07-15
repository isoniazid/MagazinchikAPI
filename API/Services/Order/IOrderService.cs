namespace MagazinchikAPI.Services
{
    public interface IOrderService
    {
        public Task CreateOrder(HttpContext context, long addressId);

        public Task<string> PayForOrder(long orderId, HttpContext context);

        public Task CheckPaymentsForOrders();

        public Task CheckPaymentForOrders(HttpContext context);
    }
}