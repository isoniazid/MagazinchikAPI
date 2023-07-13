using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class OrderEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapPost("/api/order/create", CreateOrder).WithTags("Dev");
        }

        public async Task<IResult> CreateOrder(IOrderService service, HttpContext httpContext, [FromQuery] long addressId)
        {
            await service.CreateOrder(httpContext, addressId);
            return Results.Ok();
        }
    }
}