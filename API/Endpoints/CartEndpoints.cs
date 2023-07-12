using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace API.Endpoints
{
    public class CartEndpoints
    {

        public void Define(WebApplication app)
        {
            app.MapPost("api/cart/add", AddToCart).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapDelete("api/cart/remove", RemoveFromCart).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapPut("api/cart/decrease", DecreaseFromCart).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

        }



        [Authorize]
        public async Task<IResult> AddToCart(ICartService service, [FromQuery] long productId, HttpContext context)
        {
            await service.AddToCart(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> RemoveFromCart(ICartService service, [FromQuery] long productId, HttpContext context)
        {
            await service.RemoveFromCart(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> DecreaseFromCart(ICartService service, [FromQuery] long productId, HttpContext context)
        {
            await service.DecreaseFromCart(productId, context);
            return Results.Ok();
        }
    }
}