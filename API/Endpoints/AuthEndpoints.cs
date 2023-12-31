using MagazinchikAPI.DTO.User;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class AuthEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapPost("/api/auth/register", Register)
            .Accepts<UserDtoRegistration>("application/json")
            .Produces<UserDtoAuthSuccess>(StatusCodes.Status200OK).WithTags("Auth")
            .Produces<ValidatorErrorMessage>(StatusCodes.Status422UnprocessableEntity)
            .Produces<APIErrorMessage>(StatusCodes.Status400BadRequest);

            app.MapGet("/api/auth/logout", Logout).WithTags("Auth")
            .Produces<APIErrorMessage>(401).Produces(200);

            app.MapGet("/api/auth/refresh", Refresh).WithTags("Auth")
            .Produces<APIErrorMessage>(401).Produces<UserDtoAuthSuccess>(200);

            app.MapPost("/api/auth/login", Login).WithTags("Auth")
            .Produces<UserDtoAuthSuccess>(200).Produces<APIErrorMessage>(401)
            .Produces<ValidatorErrorMessage>(StatusCodes.Status422UnprocessableEntity);
        }


        public async Task<IResult> Register(HttpContext httpContext, [FromBody] UserDtoRegistration userRegDto, ITokenService service)
        {
            return Results.Ok(await service.Register(userRegDto, httpContext));
        }

        public async Task<IResult> Refresh(HttpContext httpContext, ITokenService service)
        {
            return Results.Ok(await service.Refresh(httpContext));
        }

        public async Task<IResult> Login(HttpContext httpContext, ITokenService service, [FromBody] UserDtoLogin loggingUser)
        {
            return Results.Ok(await service.Login(httpContext, loggingUser));
        }

        public async Task<IResult> Logout(HttpContext httpContext, ITokenService service)
        {
            await service.Logout(httpContext);
            return Results.Ok();
        }



    }
}