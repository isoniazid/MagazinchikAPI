namespace MagazinchikAPI.Services.Photo
{
    public interface IPhotoService
    {
        public Task<string> GetPhoto(long id);

        public Task UploadProductPhoto(long productId, IFormFile photoToUpload);

        public Task UploadBannerPhoto(long productId, IFormFile photoToUpload);

        public Task DeleteProductPhoto(long photoId);
        public Task DeleteBannerPhoto(long photoId);
        public Task ChangeOrder(long productId, long[] photoIds);
    }
}