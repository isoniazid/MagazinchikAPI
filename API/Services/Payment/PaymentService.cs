using API.Services.Payment;
using MagazinchikAPI.Model;
using Yandex.Checkout.V3;

namespace MagazinchikAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private const string CURRENCY = "RUB";
        private readonly AsyncClient _client;
        public PaymentService(AsyncClient client)
        {
            _client = client;
        }


        public async Task CapturePayment(string paymentId) => await _client.CapturePaymentAsync(paymentId);


        public async Task<PaymentState> GetPaymentStatus(string paymentId)
        {

            var payment = await _client.GetPaymentAsync(paymentId)
            ?? throw new Exception($"Can't get this payment: {paymentId}. Maybe, it does not exist?");

            switch (payment.Status)
            {
                case PaymentStatus.WaitingForCapture:
                    if (payment.Paid) return PaymentState.READY_TO_CAPTURE;
                    return PaymentState.AWAIT_PAYMENT;
                    
                case PaymentStatus.Succeeded:
                    return PaymentState.PAID;

                case PaymentStatus.Canceled:
                    return PaymentState.CANCELLED;

                case PaymentStatus.Pending:
                    return PaymentState.AWAIT_PAYMENT;

                default:
                    return PaymentState.UNKNOWN;
            }
        }


        public async Task<(string url, string paymentId)> InitiatePayment(Order order)
        {
            var currentPayment = new NewPayment()
            {
                Description = $"Заказ № {order.Id}",
                Amount = new Amount { Value = order.Price, Currency = CURRENCY },
                Confirmation = new Confirmation
                {
                    Type = ConfirmationType.Redirect,
                    //Url указывает на эндпоинт, по которому подтверждается оплата в магазине
                    ReturnUrl = "http://localhost:3000"
                }
            };

            Payment payment = await _client.CreatePaymentAsync(currentPayment);

            string url = payment.Confirmation.ConfirmationUrl;

            return (url, payment.Id);

        }

    }
}