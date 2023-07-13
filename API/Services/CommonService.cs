using MagazinchikAPI.Infrastructure;

namespace MagazinchikAPI.Services
{
    public class CommonService
    {

        private readonly ApplicationDbContext _context;

        public CommonService(ApplicationDbContext context)
        {
            _context = context;
        }
        
         public async Task<long> UserIsOk(HttpContext context)
        {
            long jwtId = Convert.ToInt64(context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new APIException("Broken acces token", 401));
            _ = await _context.Users.FindAsync(jwtId) ?? throw new APIException("Undefined user", 401);

            return jwtId;
        }
    }
}