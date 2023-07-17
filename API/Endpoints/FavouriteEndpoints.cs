using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Favourite;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services.Favourite;

namespace MagazinchikAPI.Endpoints
{
    public class FavouriteEndpoints
    {

        public void Define(WebApplication app)
        {
            app.MapPost("api/favourite/add", AddToFavourite).WithTags("Favourite")
            .Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400).Produces(200);

            app.MapDelete("api/favourite/remove", RemoveFromFavourite).WithTags("Favourite")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapGet("api/favourite/user", GetAllFavouritesForUser).WithTags("Favourite")
            .Produces<Page<FavouriteDtoBaseInfo>>(200)
            .Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401)
            .Produces<APIErrorMessage>(400);
        }


        [Authorize]
        public async Task<IResult> AddToFavourite(IFavouriteService service, [FromQuery] long productId, HttpContext context)
        {
            await service.AddToFavourite(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> RemoveFromFavourite(IFavouriteService service, [FromQuery] long productId, HttpContext context)
        {
            await service.RemoveFromFavourite(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> GetAllFavouritesForUser(IFavouriteService service, HttpContext context, int limit, int page)
        {
            return Results.Ok(await service.GetAllFavouritesForUser(context, limit, page));
        }
        
    }
}