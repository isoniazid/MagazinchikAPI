using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class ProductEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapGet("api/product/get_all", GetAll).WithTags("Dev")
            .Produces<List<DTO.ProductDtoBaseInfo>>();

            app.MapPost("api/product/create", Create).WithTags("Admin")
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(400);

            app.MapGet("api/product/detail", GetBaseInfo).WithTags("Common")
            .Produces<DTO.ProductDtoBaseInfo>().Produces<APIErrorMessage>(404);

            app.MapGet("api/product/random_from_cathegory", GetRandomByCathegory).WithTags("Common")
            .Produces<List<DTO.ProductDtoBaseInfo>>();

            app.MapGet("api/product/random_personal", GetRandomPersonal).WithTags("User")
            .Produces<List<DTO.ProductDtoBaseInfo>>().Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400);

            app.MapGet("api/product/popular", GetPopular).WithTags("Common")
            .Produces<DTO.Page<DTO.ProductDtoBaseInfo>>().Produces<APIErrorMessage>(400);

            app.MapPost("api/product/add_to_favourite", AddToFavourite).WithTags("User")
            .Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400).Produces(200);

            app.MapDelete("api/product/remove_from_favourite", RemoveFromFavourite).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapPost("api/product/add_to_cart", AddToCart).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapDelete("api/product/remove_from_cart", RemoveFromCart).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapPut("api/product/decrease_from_cart", DecreaseFromCart).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);


        }

        public async Task<IResult> GetAll(IProductService service)
        {
            return Results.Ok(await service.GetAll());
        }

        public async Task<IResult> GetBaseInfo(IProductService service, [FromQuery] long productId)
        {
            return Results.Ok(await service.GetBaseInfo(productId));
        }

        public async Task<IResult> Create(IProductService service, [FromBody] DTO.ProductDtoCreate dto)
        {
            await service.Create(dto);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> AddToFavourite(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            await service.AddToFavourite(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> RemoveFromFavourite(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            await service.RemoveFromFavourite(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> AddToCart(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            await service.AddToCart(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> RemoveFromCart(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            await service.RemoveFromCart(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> DecreaseFromCart(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            await service.DecreaseFromCart(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> GetRandomPersonal(IProductService service, [FromQuery] int limit, HttpContext context)
        {
            return Results.Ok(await service.GetRandomPersonal(context, limit));
        }

        public IResult GetRandomByCathegory(IProductService service, [FromQuery] long cathegoryId, [FromQuery] int limit, HttpContext context)
        {
            return Results.Ok(service.GetRandomByCathegory(cathegoryId, context, limit));
        }

        public IResult GetPopular(IProductService service, [FromQuery] int limit, [FromQuery] int offset)
        {
            return Results.Ok(service.GetPopular(limit, offset));
        }

    }
}