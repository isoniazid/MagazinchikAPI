using MagazinchikAPI.Model;
using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.User;

namespace MagazinchikAPI.Services
{
public interface ITokenService
{
    string BuildAccessToken(UserDtoToken user);

    RefreshToken BuildRefreshToken(User user);

    Task<UserDtoRefresh> Refresh(HttpContext context);

    Task Logout(HttpContext context);

    Task<UserDtoLogged> Login(HttpContext httpContext, UserDtoLogin loggingUser);

    Task<UserDtoRegistered> Register(UserDtoRegistration regDto, HttpContext httpContext);
}
}