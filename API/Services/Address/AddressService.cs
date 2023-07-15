using FluentValidation;
using MagazinchikAPI.DTO.Address;
using MagazinchikAPI.Infrastructure;

namespace MagazinchikAPI.Services.Address
{
    public class AddressService : IAddressService
    {
        private const int MAX_ADDRESSES_AMOUNT = 50;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IValidator<AddressDtoCreate> _addressDtoCreateValidator;
        private readonly CommonService _commonService;
        public AddressService(IMapper mapper, ApplicationDbContext context, IValidator<AddressDtoCreate> addressDtoCreateValidator,
        CommonService commonService)
        {
            _mapper = mapper;
            _context = context;
            _addressDtoCreateValidator = addressDtoCreateValidator;
            _commonService = commonService;
        }
        public async Task<AddressDtoCreated> CreateAddress(AddressDtoCreate input, HttpContext httpContext)
        {
            var validation = _addressDtoCreateValidator.Validate(input);
            if (!validation.IsValid) throw new ValidatorException(validation);

            var jwtId = await _commonService.UserIsOk(httpContext);

            if(await _context.Addresses.Where(x => x.UserId == jwtId).CountAsync() >= MAX_ADDRESSES_AMOUNT)
            throw new APIException($"Limit exceeded. Max amount {MAX_ADDRESSES_AMOUNT}", 400);

            var addressToSave = _mapper.Map<Model.Address>(input);
            addressToSave.UserId = jwtId;

            await _context.Addresses.AddAsync(addressToSave);
            await _context.SaveChangesAsync();

            return _mapper.Map<AddressDtoCreated>(addressToSave);
        }

        public async Task<List<AddressDtoBaseInfo>> GetAllForUser(HttpContext context)
        {
            var jwtId = await _commonService.UserIsOk(context);

            var result = await _context.Addresses.Where(x => x.UserId == jwtId).ProjectTo<AddressDtoBaseInfo>(_mapper.ConfigurationProvider).ToListAsync();

            return result;
        }
    }
}