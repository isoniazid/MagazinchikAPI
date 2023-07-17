using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services.Photo;

namespace MagazinchikAPI.Endpoints
{
    public class PhotoEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapGet("api/photo", GetPhoto).WithTags("Photo")
            .Produces(200).Produces<APIErrorMessage>(404);

            app.MapPost("api/photo/product/upload", UploadProductPhoto).WithTags("Photo")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(400);

            app.MapPost("api/photo/banner/upload", UploadBannerPhoto).WithTags("Photo")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(400);

            app.MapDelete("api/photo/banner/delete", DeleteBannerPhoto).WithTags("Photo")
            .Produces(200).Produces<APIErrorMessage>(404);

            app.MapDelete("api/photo/product/delete", DeleteProductPhoto).WithTags("Photo")
            .Produces(200).Produces<APIErrorMessage>(404);

            app.MapPut("api/photo/product/change-order", ChangeOrder).WithTags("Photo")
             .Produces(200).Produces<APIErrorMessage>(400);
        }

        public async Task<IResult> GetPhoto(IPhotoService service, [FromQuery] long photoId)
        {
            return Results.File(await service.GetPhoto(photoId), contentType: "image/jpg");
        }


        public async Task<IResult> UploadProductPhoto(IPhotoService service, [FromQuery] long productId, IFormFile photo)
        {
            await service.UploadProductPhoto(productId, photo);
            return Results.Ok();
        }

        public async Task<IResult> UploadBannerPhoto(IPhotoService service, [FromQuery] long bannerId, IFormFile photo)
        {
            await service.UploadBannerPhoto(bannerId, photo);
            return Results.Ok();
        }

        public async Task<IResult> DeleteProductPhoto(IPhotoService service, [FromQuery] long photoId)
        {
            await service.DeleteProductPhoto(photoId);
            return Results.Ok();
        }

        
        public async Task<IResult> DeleteBannerPhoto(IPhotoService service, [FromQuery] long photoId)
        {
            await service.DeleteBannerPhoto(photoId);
            return Results.Ok();
        }

        public async Task<IResult> ChangeOrder(IPhotoService service, [FromQuery] long productId, [FromBody] long[] newOrder)
        {
            await service.ChangeOrder(productId, newOrder);
            return Results.Ok();
        }
    }
}