using API.Services.Payment;

namespace MagazinchikAPI.Services
{
    public interface IPaymentService
    {
        public Task<PaymentState> GetPaymentStatus(string paymentId);
        
        public Task CapturePayment(string paymentId);
        public Task<(string url, string paymentId)> InitiatePayment(Model.Order order);
    }
}