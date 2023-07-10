using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services.Photo;

namespace MagazinchikAPI.Endpoints
{
    public class PhotoEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapGet("api/photo", GetPhoto).WithTags("Common")
            .Produces(200).Produces<APIErrorMessage>(404);

            app.MapPost("api/photo/upload", UploadPhoto).WithTags("Admin")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(400);

            app.MapDelete("api/photo/delete", DeletePhoto).WithTags("Admin")
            .Produces(200).Produces<APIErrorMessage>(404);

            app.MapPut("api/photo/change_order", ChangeOrder).WithTags("Admin")
             .Produces(200).Produces<APIErrorMessage>(400);
        }

        public async Task<IResult> GetPhoto(IPhotoService service, [FromQuery] long photoId)
        {
            return Results.File(await service.GetPhoto(photoId), contentType: "image/jpg");
        }


        public async Task<IResult> UploadPhoto(IPhotoService service, [FromQuery] long productId, IFormFile photo)
        {
            await service.UploadProductPhoto(productId, photo);
            return Results.Ok();
        }

        public async Task<IResult> DeletePhoto(IPhotoService service, [FromQuery] long photoId)
        {
            await service.DeleteProductPhoto(photoId);
            return Results.Ok();
        }

        public async Task<IResult> ChangeOrder(IPhotoService service, [FromQuery] long productId, [FromBody] long[] newOrder)
        {
            await service.ChangeOrder(productId, newOrder);
            return Results.Ok();
        }
    }
}