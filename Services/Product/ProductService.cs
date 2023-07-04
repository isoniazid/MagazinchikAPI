using MagazinchikAPI.DTO;
using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;

namespace MagazinchikAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task Create(ProductDtoCreate input)
        {   
            await _context.Products.AddAsync(_mapper.Map<Product>(input));
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductDtoBaseInfo>> GetAll()
        {
            return await _context.Products.ProjectTo<ProductDtoBaseInfo>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}