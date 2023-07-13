namespace MagazinchikAPI.Services
{
    public interface IOrderService
    {
        public Task CreateOrder(HttpContext context, long addressId);
    }
}