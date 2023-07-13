using MagazinchikAPI.DTO.User;
using MagazinchikAPI.Model;
using MagazinchikAPI.Infrastructure;
using FluentValidation;

namespace MagazinchikAPI.Services
{
    public class TokenService : ITokenService
    {

        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IValidator<UserDtoRegistration> _registrationValidator;
        private readonly IValidator<UserDtoLogin> _loginValidator;

        public TokenService(IMapper mapper, ApplicationDbContext context,
        IValidator<UserDtoRegistration> registrationValidator, IValidator<UserDtoLogin> loginValidator)
        {
            _mapper = mapper;
            _context = context;
            _registrationValidator = registrationValidator;
            _loginValidator = loginValidator;
        }

        public string BuildAccessToken(UserDtoToken user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var secureKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Starter.JWT_KEY));
            var credentials = new SigningCredentials(secureKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new JwtSecurityToken(Starter.JWT_ISSUER, Starter.JWT_ISSUER, claims,
            expires: DateTime.UtcNow.Add(Starter.AccessTokenTime), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public RefreshToken BuildRefreshToken(User user)
        {
            var rtokenStr = Encoding.UTF8.GetBytes(user.Email + user.Id + DateTime.UtcNow.ToString());
            var rTokenSeed = new byte[32];
            new Random().NextBytes(rTokenSeed);

            var rTokenVal = rtokenStr.Concat(rTokenSeed).ToArray();

            var encrypter = new System.Security.Cryptography.HMACSHA256
            { Key = Starter.REFRESH_HASH_KEY };

            return new RefreshToken
            {
                Value = Convert.ToBase64String(encrypter.ComputeHash(rTokenVal)),
                User = user,
                Expires = DateTime.UtcNow+Starter.RefreshTokenTime
            };
        }

        public async Task<UserDtoRefresh> Refresh(HttpContext httpContext)
        {
            string refreshTokenFromCookies = httpContext.Request.Cookies["refresh_token"] ?? throw new APIException("Отсутствует RefreshToken в cookie", StatusCodes.Status401Unauthorized);

            var currentRefreshToken = await _context.RefreshTokens.Include(p => p.User)
            .FirstOrDefaultAsync(x => x.Value == refreshTokenFromCookies)
            ?? throw new APIException("Отправленный Refresh Token отсутствует в БД", 401);

            //Если токен сгорел
            if (currentRefreshToken.Expires < DateTime.UtcNow)
                throw new APIException($"RefreshToken истек. Валиден до: {currentRefreshToken.Expires}", StatusCodes.Status401Unauthorized);

            //Если токен скоро сгорит...
            if (currentRefreshToken.Expires - Starter.RefreshTokenThreshold < DateTime.UtcNow)
            {
                currentRefreshToken = BuildRefreshToken(currentRefreshToken.User);
                await _context.RefreshTokens.AddAsync(currentRefreshToken);
                await _context.SaveChangesAsync();
                SaveToCookies(httpContext, currentRefreshToken);
            }

           

            var result = _mapper.Map<UserDtoRefresh>(currentRefreshToken.User);

            result.AccessToken = BuildAccessToken(_mapper.Map<UserDtoToken>(currentRefreshToken.User));

            return result;

        }

        public async Task<UserDtoLogged> Login(HttpContext httpContext, UserDtoLogin loggingUser)
        {
            var validation = _loginValidator.Validate(loggingUser);
            if (!validation.IsValid) throw new ValidatorException(validation);

            loggingUser.Password = loggingUser.HashPassword(loggingUser.Password, loggingUser.Email);

            var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Password == loggingUser.Password && x.Email == loggingUser.Email)
            ?? throw new APIException("Incorrect login or password", 401);

            var accessToken = BuildAccessToken(_mapper.Map<UserDtoToken>(existingUser));

            var refreshToken = BuildRefreshToken(existingUser);
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            SaveToCookies(httpContext, refreshToken);

            var result = _mapper.Map<UserDtoLogged>(existingUser);
            result.AccessToken = accessToken;

            return result;
        }


        public async Task<UserDtoRegistered> Register(UserDtoRegistration regDto, HttpContext httpContext)
        {
            var validation = _registrationValidator.Validate(regDto);
            if (!validation.IsValid) throw new ValidatorException(validation);



            var userToSave = _mapper.Map<User>(regDto);
            (userToSave.UpdatedAt, userToSave.CreatedAt) = (DateTime.UtcNow, DateTime.UtcNow);

            await _context.Users.AddAsync(userToSave);
            await _context.SaveChangesAsync();


            var userDto = new UserDtoToken
            { Email = userToSave.Email, Id = userToSave.Id, Role = userToSave.Role };

            var accessToken = BuildAccessToken(userDto);
            var refreshToken = BuildRefreshToken(userToSave);
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            SaveToCookies(httpContext, refreshToken);

            var result = _mapper.Map<UserDtoRegistered>(userToSave);
            result.AccessToken = accessToken;
            return result;
        }

        public async Task Logout(HttpContext httpContext)
        {
            string refreshTokenFromCookies = httpContext.Request.Cookies["refresh_token"]
          ?? throw new APIException("Невозможно удалить RefreshToken, т.к. он отсутствует в cookie", StatusCodes.Status401Unauthorized);
            httpContext.Response.Cookies.Delete("refresh_token");


            
            var currentUserRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Value == refreshTokenFromCookies);

            await DeleteUserRefreshTokensFromDb(currentUserRefreshToken!.UserId);
            
            await _context.SaveChangesAsync();
        }

        private async Task DeleteUserRefreshTokensFromDb(long userId)
        {
            var refreshTokensToDelete = await _context.RefreshTokens.Where(x => x.UserId == userId).ToListAsync();
            _context.RemoveRange(refreshTokensToDelete);
            await _context.SaveChangesAsync();
        }

        private static void SaveToCookies(HttpContext context, RefreshToken token)
        {
            if (context.Request.Cookies.ContainsKey("refresh_token"))
            {
                context.Response.Cookies.Delete("refresh_token");
            }

            context.Response.Cookies.Append("refresh_token", token.Value, new CookieOptions() { Secure = true, HttpOnly = true, MaxAge = new TimeSpan(45, 0, 0, 0) });
        }
    }
}