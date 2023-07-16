using FluentValidation;
using MagazinchikAPI.DTO.Banner;
using MagazinchikAPI.Infrastructure;

namespace MagazinchikAPI.Services.Banner
{
    public class BannerService : IBannerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<BannerDtoCreate> _bannerCreateValidator;
        private readonly CommonService _commonService;
        public BannerService(ApplicationDbContext context, IMapper mapper,
         IValidator<BannerDtoCreate> bannerCreateValidator, CommonService commonService)
        {
            _commonService = commonService;
            _context = context;
            _mapper = mapper;
            _bannerCreateValidator = bannerCreateValidator;
        }

        public async Task Create(BannerDtoCreate input)
        {
            var validation = _bannerCreateValidator.Validate(input);
            if (!validation.IsValid) throw new ValidatorException(validation);

            _ = _context.Banners.FirstOrDefaultAsync(x => x.Name == input.Name)
            ?? throw new APIException("banner with such name already exists", 400);

            var bannerToSave = _mapper.Map<Model.Banner>(input);

            await _context.Banners.AddAsync(bannerToSave);

            await _context.SaveChangesAsync();
        }
    }
}