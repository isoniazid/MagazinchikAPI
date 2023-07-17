using MagazinchikAPI.Infrastructure;

namespace MagazinchikAPI.Services.Photo
{
    public class PhotoService : IPhotoService
    {
        private const long MAX_PHOTO_SIZE = 1048576; //1MB
        private readonly ApplicationDbContext _context;
        public PhotoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetPhoto(long id)
        {
            var photoToGet = await _context.Photos.FindAsync(id)
            ?? throw new APIException("Photo doesn't exist", 404);

            var path = $"{Directory.GetCurrentDirectory()}/img/{photoToGet.ProductId}/{photoToGet.FileName}.jpg";

            return path;

        }

        public async Task UploadProductPhoto(long productId, IFormFile photoToUpload)
        {
            _ = await _context.Products.FindAsync(productId)
                ?? throw new APIException("No such product", 404);

            if (photoToUpload.Length > MAX_PHOTO_SIZE)
                throw new APIException($"Photo size is too big. Mustn't be bigger than {MAX_PHOTO_SIZE / 1048576}MB", 400);

            var photoFormat = Path.GetExtension(photoToUpload.FileName);
            if (!string.Equals(photoFormat, ".jpg", StringComparison.OrdinalIgnoreCase))
                throw new APIException("Incorrect photo format. Must be JPEG", 400);


            var photosAmount = _context.Photos.Where(x => x.ProductId == productId).Count();

            var photoOrder = photosAmount > 0 ? photosAmount - 1 : 0;


            var photoToDb = new Model.Photo()
            {
                PhotoOrder = photoOrder,
                FileName = Guid.NewGuid().ToString(),
                ProductId = productId
            };

            await _context.AddAsync(photoToDb);

            if(!Directory.Exists($"{Directory.GetCurrentDirectory()}/img/{productId}")) 
            Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/img/{productId}");

            string uploadPath = $"{Directory.GetCurrentDirectory()}/img/{productId}/{photoToDb.FileName}.jpg";

            using var fileStream = new FileStream(uploadPath, FileMode.Create);
            await photoToUpload.CopyToAsync(fileStream);

            await _context.SaveChangesAsync();
        }

        public async Task UploadBannerPhoto(long bannerId, IFormFile photoToUpload)
        {
            _ = await _context.Banners.FindAsync(bannerId)
                ?? throw new APIException("No such banner", 404);

                
            if (photoToUpload.Length > MAX_PHOTO_SIZE)
                throw new APIException($"Photo size is too big. Mustn't be bigger than {MAX_PHOTO_SIZE / 1048576}MB", 400);

            var photoFormat = Path.GetExtension(photoToUpload.FileName);
            if (!string.Equals(photoFormat, ".jpg", StringComparison.OrdinalIgnoreCase))
                throw new APIException("Incorrect photo format. Must be JPEG", 400);

            //NB Platform-dependent: for windows only
            /* using( var image = Image.FromStream(photoToUpload.OpenReadStream()))
            {
                var aspectRatio = (float)image.Width/(float)image.Height;
                if(aspectRatio > 2.1 || aspectRatio < 1.9) throw new APIException("Invalid aspect ratio: Must be close to 2:1", 400);
            } */


            var photosAmount = _context.Photos.Where(x => x.BannerId == bannerId).Count();

            var photoOrder = photosAmount > 0 ? photosAmount - 1 : 0;


            var photoToDb = new Model.Photo()
            {
                PhotoOrder = photoOrder,
                FileName = Guid.NewGuid().ToString(),
                BannerId = bannerId
            };

            await _context.Photos.AddAsync(photoToDb);

            if(!Directory.Exists($"{Directory.GetCurrentDirectory()}/img/banners/{bannerId}")) 
            Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/img/banners/{bannerId}");

            string uploadPath = $"{Directory.GetCurrentDirectory()}/img/banners/{bannerId}/{photoToDb.FileName}.jpg";

            using var fileStream = new FileStream(uploadPath, FileMode.Create);
            await photoToUpload.CopyToAsync(fileStream);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductPhoto(long photoId)
        { //NB no rearrange after deleting
            var photoToDelete = await _context.Photos.FindAsync(photoId)
            ?? throw new APIException("Nothing to delete", 404);

            File.Delete($"{Directory.GetCurrentDirectory()}/img/{photoToDelete.ProductId}/{photoToDelete.FileName}.jpg");
            _context.Remove(photoToDelete);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBannerPhoto(long photoId)
        { //NB no rearrange after deleting
            var photoToDelete = await _context.Photos.FindAsync(photoId)
            ?? throw new APIException("Nothing to delete", 404);

            File.Delete($"{Directory.GetCurrentDirectory()}/img/banners/{photoToDelete.BannerId}/{photoToDelete.FileName}.jpg");
            _context.Remove(photoToDelete);

            await _context.SaveChangesAsync();
        }

        public async Task ChangeOrder(long productId, long[] photoIds)
        {
            var photosToChangeOrder = await _context.Photos.Where(x => x.ProductId == productId)
            .OrderBy(x => x.PhotoOrder).ToListAsync();

            if (photoIds.Length != photosToChangeOrder.Count)
                throw new APIException($"Incorrect input amount: {photoIds.Length}. Must be: {photosToChangeOrder.Count}", 400);

            for (int i = 0; i < photoIds.Length; ++i)
            {
                var currentPhoto = photosToChangeOrder.FirstOrDefault(x => x.Id == photoIds[i])
                ?? throw new APIException($"No photo with Id {photoIds[i]} found", 400);
                currentPhoto.PhotoOrder = i;
            }

            await _context.SaveChangesAsync();

        }
    }
}