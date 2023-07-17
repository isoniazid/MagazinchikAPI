using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.DTO.Favourite;
using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Services.Favourite
{
    public class FavouriteService : IFavouriteService
    {
        private const int LIMIT_SIZE = 50;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly CommonService _commonService;

        public FavouriteService(ApplicationDbContext context, IMapper mapper, CommonService commonService)
        {
            _commonService = commonService;
            _context = context;
            _mapper = mapper;
        }

        public async Task AddToFavourite(long productId, HttpContext context)
        {

            var jwtId = await _commonService.UserIsOk(context);


            if (_context.Favourites.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId) != null)
                throw new APIException("Already added to favourites", 400);

            _ = await _context.Products.FindAsync(productId)
            ?? throw new APIException("Product does not exist", 404);


            var FavouriteToSave = new Model.Favourite { ProductId = productId, UserId = jwtId };

            await _context.Favourites.AddAsync(FavouriteToSave);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromFavourite(long productId, HttpContext context)
        {
            var jwtId = await _commonService.UserIsOk(context);

            var favouriteToRemove = _context.Favourites.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId)
            ?? throw new APIException("Nothing to remove", 404);

            _context.Favourites.Remove(favouriteToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<Page<FavouriteDtoBaseInfo>> GetAllFavouritesForUser(HttpContext context, int limit, int offset)
        {
            if (limit > LIMIT_SIZE) throw new APIException($"Too big amount for one query: {limit}", 400);

            var jwtId = await _commonService.UserIsOk(context);

            var elementsCount = _context.Favourites.Where(x => x.UserId == jwtId).Count();

            var pages = Page.CalculatePagesAmount(elementsCount, limit);
            if (!Page.OffsetIsOk(offset, pages)) throw new APIException($"Invalid offset: {offset}", 400);

            var productsFromFavourites = await _context.Favourites
            .Include(x => x.Product)
            .ThenInclude(y => y!.Photos)
            .Where(x => x.UserId == jwtId)
            .ProjectTo<FavouriteDtoBaseInfo>(_mapper.ConfigurationProvider).ToListAsync();

            return new Page<FavouriteDtoBaseInfo>()
            { CurrentOffset = offset, CurrentPage = productsFromFavourites, Pages = pages, ElementsCount = elementsCount };
        }
    }
}