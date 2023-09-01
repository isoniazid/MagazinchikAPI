using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Model;
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
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(404)
            .Produces<APIErrorMessage>(400).Produces(403);

            app.MapGet("api/product/detail", GetDetailedInfo).WithTags("Product")
            .Produces<DTO.ProductDtoBaseInfo>().Produces<APIErrorMessage>(404);

            app.MapGet("api/product/random", GetRandom).WithTags("Product")
            .Produces<List<DTO.ProductDtoBaseInfo>>();

            app.MapGet("api/product/random-from-category", GetRandomByCategory).WithTags("Product")
            .Produces<List<DTO.ProductDtoBaseInfo>>();

            app.MapGet("api/product/personal", GetRandomPersonal).WithTags("Product")
            .Produces<List<DTO.ProductDtoBaseInfo>>().Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400);

            app.MapGet("api/product/popular", GetPopular).WithTags("Product")
            .Produces<DTO.Page<DTO.ProductDtoBaseInfo>>().Produces<APIErrorMessage>(400);

            app.MapGet("api/product/category", GetByCategory).WithTags("Product")
            .Produces<DTO.Page<DTO.ProductDtoBaseInfo>>().Produces<APIErrorMessage>(400);

        }

        public async Task<IResult> GetAll(IProductService service, HttpContext context, [FromQuery] int limit, [FromQuery] int page)
        {
            return Results.Ok(await service.GetAll(limit, page, context));
        }

        public async Task<IResult> GetDetailedInfo(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            return Results.Ok(await service.GetDetailedInfo(productId, context));
        }

         [Authorize(Roles = "ADMIN")]
        public async Task<IResult> Create(IProductService service, [FromBody] DTO.ProductDtoCreate dto)
        {
            await service.Create(dto);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> GetRandomPersonal(IProductService service, [FromQuery] int count, HttpContext context)
        {
            return Results.Ok(await service.GetRandomPersonal(context, count));
        }

        public async Task<IResult> GetRandom(IProductService service, [FromQuery] int count, HttpContext context)
        {
            return Results.Ok(await service.GetRandom(context, count));
        }

        public async Task<IResult> GetRandomByCategory(IProductService service, [FromQuery] long categoryId, [FromQuery] int count, HttpContext context)
        {
            return Results.Ok(await service.GetRandomByCategory(categoryId, context, count));
        }

        public async Task<IResult> GetPopular(IProductService service, [FromQuery] int limit, [FromQuery] int page, HttpContext context)
        {
            return Results.Ok(await service.GetPopular(limit, page, context));
        }

        public async Task<IResult> GetByCategory(IProductService service, [FromQuery] int limit, [FromQuery] int page, [FromQuery] long categoryId, HttpContext context)
        {
            return Results.Ok(await service.GetByCategory(categoryId, limit, page, context));
        }

    }
}