using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Order;

namespace MagazinchikAPI.Services
{
    public interface IOrderService
    {
        public Task<long> CreateOrder(HttpContext context, long addressId);

        public Task<string> PayForOrder(long orderId, HttpContext context);

        public Task CheckPaymentsForOrders();

        public Task<OrderDtoBaseInfo> GetById(HttpContext context, long orderId);

        public Task<Page<OrderDtoBaseInfo>> GetAllForUser(HttpContext context, int limit, int offset);

        public Task CheckPaymentForOrders(HttpContext context);
    }
}