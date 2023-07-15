using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class OrderEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapPost("/api/order/create", CreateOrder).WithTags("Dev/Order");

            app.MapPost("api/order/pay", PayForOrder).WithTags("Dev/Order")
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(404)
            .Produces<APIErrorMessage>(400).Produces(200);

            //Get потому что на нее будет редирект
            app.MapGet("api/order/check_payment", CheckPayments).WithTags("Dev/Order")
            .Produces<APIErrorMessage>(401).Produces(200);
        }

        public async Task<IResult> CreateOrder(IOrderService service, HttpContext httpContext, [FromQuery] long addressId)
        {
            await service.CreateOrder(httpContext, addressId);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> PayForOrder(IOrderService service, [FromQuery] long orderId, HttpContext context)
        {
            return Results.Ok(await service.PayForOrder(orderId, context));
        }

        [Authorize]
        public async Task<IResult> CheckPayments(IOrderService service, HttpContext context)
        {
            await service.CheckPaymentForOrders(context);
            return Results.Ok();
        }
    }
}