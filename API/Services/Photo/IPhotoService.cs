namespace MagazinchikAPI.Services.Photo
{
    public interface IPhotoService
    {
        public Task<string> GetPhoto(long id);

        public Task UploadProductPhoto(long productId, IFormFile photoToUpload);

        public Task DeleteProductPhoto(long photoId);

        public Task ChangeOrder(long productId, long[] photoIds);
    }
}