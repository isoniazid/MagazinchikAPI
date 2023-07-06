using MagazinchikAPI.DTO;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class CathegoryEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapPost("api/cathegory/create", CreateCathegory).WithTags("Admin")
            .Produces<CathegoryDtoCreated>(200)
            .Produces<ValidatorErrorMessage>(StatusCodes.Status422UnprocessableEntity)
            .Produces<APIErrorMessage>(400);
        }

                public async Task<IResult> CreateCathegory(ICathegoryService service, [FromBody] CathegoryDtoCreate dto)
        {
            return Results.Ok(await service.CreateCathegory(dto));
        }
    }
}