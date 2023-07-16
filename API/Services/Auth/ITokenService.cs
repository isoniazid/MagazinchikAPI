using MagazinchikAPI.Model;
using MagazinchikAPI.DTO.User;

namespace MagazinchikAPI.Services
{
public interface ITokenService
{
    string BuildAccessToken(UserDtoToken user);

    RefreshToken BuildRefreshToken(User user);

    Task<UserDtoAuthSuccess> Refresh(HttpContext context);

    Task Logout(HttpContext context);

    Task<UserDtoAuthSuccess> Login(HttpContext httpContext, UserDtoLogin loggingUser);

    Task<UserDtoAuthSuccess> Register(UserDtoRegistration regDto, HttpContext httpContext);
}
}