using MagazinchikAPI.DTO.Banner;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services.Banner;

namespace MagazinchikAPI.Endpoints
{
    public class BannerEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapPost("/api/banner/create", CreateBanner).WithTags("Admin")
            .Produces(StatusCodes.Status200OK)
            .Produces<ValidatorErrorMessage>(StatusCodes.Status422UnprocessableEntity)
            .Produces<APIErrorMessage>(400);
        }

        public async Task<IResult> CreateBanner(IBannerService service, [FromBody] BannerDtoCreate input)
        {
            await service.Create(input);
            return Results.Ok();
        }
    }
}