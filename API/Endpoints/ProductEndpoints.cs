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

            app.MapGet("api/product/detail", GetDetailedInfo).WithTags("Product")
            .Produces<DTO.ProductDtoBaseInfo>().Produces<APIErrorMessage>(404);

            app.MapGet("api/product/random_from_cathegory", GetRandomByCathegory).WithTags("Product")
            .Produces<List<DTO.ProductDtoBaseInfo>>();

            app.MapGet("api/product/random_personal", GetRandomPersonal).WithTags("Product")
            .Produces<List<DTO.ProductDtoBaseInfo>>().Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400);

            app.MapGet("api/product/popular", GetPopular).WithTags("Product")
            .Produces<DTO.Page<DTO.ProductDtoBaseInfo>>().Produces<APIErrorMessage>(400);

            app.MapPost("api/product/add_to_favourite", AddToFavourite).WithTags("Product")
            .Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400).Produces(200);

            app.MapDelete("api/product/remove_from_favourite", RemoveFromFavourite).WithTags("Product")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);
        }

        public async Task<IResult> GetAll(IProductService service, HttpContext context)
        {
            return Results.Ok(await service.GetAll(context));
        }

        public async Task<IResult> GetDetailedInfo(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            return Results.Ok(await service.GetDetailedInfo(productId, context));
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
        public async Task<IResult> GetRandomPersonal(IProductService service, [FromQuery] int limit, HttpContext context)
        {
            return Results.Ok(await service.GetRandomPersonal(context, limit));
        }

        public async Task<IResult> GetRandomByCathegory(IProductService service, [FromQuery] long cathegoryId, [FromQuery] int limit, HttpContext context)
        {
            return Results.Ok(await service.GetRandomByCathegory(cathegoryId, context, limit));
        }

        public async Task<IResult> GetPopular(IProductService service, [FromQuery] int limit, [FromQuery] int page, HttpContext context)
        {
            return Results.Ok(await service.GetPopular(limit, page, context));
        }

    }
}