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


            if (_context.Favourites.FirstOrDefault(x => x.ProductId == productId && x.UserId == jwtId) is not null)
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
            if(pages <= 0) return new Page<FavouriteDtoBaseInfo>();
            if (!Page.OffsetIsOk(offset, pages)) throw new APIException($"Invalid offset: {offset}", 400);

            var favourites = await _context.Favourites
            .Include(x => x.Product)
            .Include(x => x.Product!.CartProducts)
            .Include(x => x.Product!.Photos)
            .Where(x => x.UserId == jwtId)
            .OrderByDescending(x => x.Id)
            .Skip(offset * limit)
            .Take(limit).ToListAsync();
            
            var products = favourites.Select(x => x.Product);

            var result = _mapper.Map<List<FavouriteDtoBaseInfo>>(favourites);

            //setting cart flags
            result.ForEach(x => x.Product!.SetCart(products.FirstOrDefault(y => x.Product!.Id == y!.Id)!, jwtId));

            return new Page<FavouriteDtoBaseInfo>()
            { CurrentOffset = offset, CurrentPage = result, Pages = pages, ElementsCount = elementsCount };
        }
    }
}