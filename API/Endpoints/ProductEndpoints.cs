using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Favourite;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class ProductEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapGet("api/product/get-all", GetAll).WithTags("Product")
            .Produces<DTO.Page<DTO.ProductDtoBaseInfo>>();

            app.MapPost("api/product/create", Create).WithTags("Admin")
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(400);

            app.MapGet("api/product/detail", GetDetailedInfo).WithTags("Product")
            .Produces<DTO.ProductDtoBaseInfo>().Produces<APIErrorMessage>(404);

            app.MapGet("api/product/random-from-cathegory", GetRandomByCathegory).WithTags("Product")
            .Produces<List<DTO.ProductDtoBaseInfo>>();

            app.MapGet("api/product/personal", GetRandomPersonal).WithTags("Product")
            .Produces<List<DTO.ProductDtoBaseInfo>>().Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400);

            app.MapGet("api/product/popular", GetPopular).WithTags("Product")
            .Produces<DTO.Page<DTO.ProductDtoBaseInfo>>().Produces<APIErrorMessage>(400);

            app.MapPost("api/favourite/add", AddToFavourite).WithTags("Favourite")
            .Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400).Produces(200);

            app.MapDelete("api/favourite/remove", RemoveFromFavourite).WithTags("Favourite")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapGet("api/favourite/user", GetAllFavouritesForUser).WithTags("Favourite")
            .Produces<Page<FavouriteDtoBaseInfo>>(200)
            .Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401)
            .Produces<APIErrorMessage>(400);

        }

        public async Task<IResult> GetAll(IProductService service, HttpContext context, [FromQuery] int limit, [FromQuery] int page)
        {
            return Results.Ok(await service.GetAll(limit, page, context));
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
        public async Task<IResult> GetAllFavouritesForUser(IProductService service, HttpContext context, int limit, int page)
        {
            return Results.Ok(await service.GetAllFavouritesForUser(context, limit, page));
        }

        [Authorize]
        public async Task<IResult> GetRandomPersonal(IProductService service, [FromQuery] int count, HttpContext context)
        {
            return Results.Ok(await service.GetRandomPersonal(context, count));
        }

        public async Task<IResult> GetRandomByCathegory(IProductService service, [FromQuery] long cathegoryId, [FromQuery] int count, HttpContext context)
        {
            return Results.Ok(await service.GetRandomByCathegory(cathegoryId, context, count));
        }

        public async Task<IResult> GetPopular(IProductService service, [FromQuery] int limit, [FromQuery] int page, HttpContext context)
        {
            return Results.Ok(await service.GetPopular(limit, page, context));
        }

    }
}