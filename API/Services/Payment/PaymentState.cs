namespace MagazinchikAPI.Services.Payment
{
    public enum PaymentState
    {
        READY_TO_CAPTURE,
        PAID,
        AWAIT_PAYMENT,
        CANCELLED,
        UNKNOWN
    }
}