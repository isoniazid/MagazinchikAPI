using MagazinchikAPI.DTO.Address;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services.Address;

namespace MagazinchikAPI.Endpoints
{
    public class AddressEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapGet("/api/address/all_user", GetAllForUser).WithTags("Address")
            .Produces<List<AddressDtoBaseInfo>>(200).Produces<APIErrorMessage>(401);

            app.MapPost("/api/address/create", CreateAddress).WithTags("Address")
            .Produces<AddressDtoCreated>(200).Produces<ValidatorErrorMessage>(StatusCodes.Status422UnprocessableEntity)
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400);
            
        }

        [Authorize]
        public async Task<IResult> CreateAddress(IAddressService service, HttpContext context, [FromBody] AddressDtoCreate input)
        {
            return Results.Ok(await service.CreateAddress(input, context));
        }

        [Authorize]
        public async Task<IResult> GetAllForUser(IAddressService service, HttpContext context)
        {
            return Results.Ok(await service.GetAllForUser(context));
        }
    }
}