using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.CartProduct;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class CartEndpoints
    {

        public void Define(WebApplication app)
        {
            app.MapPost("api/cart/add", AddToCart).WithTags("Cart")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapDelete("api/cart/remove", RemoveFromCart).WithTags("Cart")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapPut("api/cart/decrease", DecreaseFromCart).WithTags("Cart")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapGet("api/cart/user", GetAllForUser).WithTags("Cart")
            .Produces<Page<CartProductDtoBaseInfo>>(200)
            .Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401)
            .Produces<APIErrorMessage>(400);

            app.MapPatch("api/cart/set", SetCertainAmount).WithTags("Cart")
            .Produces(200)
            .Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401)
            .Produces<APIErrorMessage>(400);

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

        [Authorize]
        public async Task<IResult> GetAllForUser(ICartService service, HttpContext context, [FromQuery] int limit, [FromQuery] int page)
        {
            return Results.Ok(await service.GetAllForUser(context, limit, page));
        }

        [Authorize]
        public async Task<IResult> SetCertainAmount(ICartService service, HttpContext context, [FromQuery] long productId, [FromQuery] long count)
        {
            await service.SetToCertainAmount(productId, count, context);
            return Results.Ok();
        }
    }
}