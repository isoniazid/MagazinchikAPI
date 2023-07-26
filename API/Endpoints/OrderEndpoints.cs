using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Order;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class OrderEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapPost("/api/order/create", CreateOrder).WithTags("Order")
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(404)
            .Produces<long>(200);

            app.MapPost("api/order/pay", PayForOrder).WithTags("Order")
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(404)
            .Produces<APIErrorMessage>(400).Produces<OrderPaymentDto>(200);

            //Get потому что на нее будет редирект
            app.MapGet("api/order/check-payment", CheckPayments).WithTags("Dev/Order")
            .Produces<APIErrorMessage>(401).Produces(200);

            app.MapGet("api/order", GetById).WithTags("Order")
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(404)
            .Produces<OrderDtoBaseInfo>(200);

            app.MapGet("api/order/user", GetAllForUser).WithTags("Order")
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(404)
            .Produces<Page<OrderDtoBaseInfo>>(200).Produces<APIErrorMessage>(400);


        }

        public async Task<IResult> CreateOrder(IOrderService service, HttpContext httpContext, [FromQuery] long addressId)
        {
            
            return Results.Ok(await service.CreateOrder(httpContext, addressId));
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

        [Authorize]
        public async Task<IResult> GetById(IOrderService service, HttpContext context, long orderId)
        {
            return Results.Ok(await service.GetById(context, orderId));
        }

        [Authorize]
        public async Task<IResult> GetAllForUser(IOrderService service,HttpContext context, int limit, int page)
        {
            return Results.Ok(await service.GetAllForUser(context, limit, page));
        }
    }
}