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


            app.MapGet("api/cathegory/random", GetRandomCathegories).WithTags("Common")
            .Produces<CathegoryDtoCreated>(200)
            .Produces<APIErrorMessage>(400);
        }

        public async Task<IResult> CreateCathegory(ICathegoryService service, [FromBody] CathegoryDtoCreate dto)
        {
            return Results.Ok(await service.CreateCathegory(dto));
        }

        public IResult GetRandomCathegories(ICathegoryService service, [FromQuery] int count)
        {
            return Results.Ok(service.GetRandomCathegories(count));
        }
    }
}