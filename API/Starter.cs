using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Endpoints;
using MagazinchikAPI.Services;
using MagazinchikAPI.Validators;
using FluentValidation;
using MagazinchikAPI.Services.Photo;
using Yandex.Checkout.V3;
using Quartz;
using Quartz.AspNetCore;
using MagazinchikAPI.Services.Jobs;
using MagazinchikAPI.Services.Address;
using MagazinchikAPI.Services.Banner;
using MagazinchikAPI.Services.Favourite;
using MagazinchikAPI.Services.CacheWrapper;

public static class Starter
{
    private static string SHOP_ID = string.Empty;
    private static string SHOP_SECRET_KEY = string.Empty;
    public static byte[] PASSWORD_HASH_KEY { get; private set; } = new byte[1];
    public static byte[] REFRESH_HASH_KEY { get; private set; } = new byte[1];
    public static string JWT_ISSUER { get; private set; } = string.Empty;
    public static string JWT_KEY { get; private set; } = string.Empty;
    public static string JWT_AUDIENCE { get; private set; } = string.Empty;

    public static readonly TimeSpan AccessTokenTime = new(0, 30, 0);
    public static readonly TimeSpan RefreshTokenTime = new(30, 0, 0, 0);
    public static readonly TimeSpan RefreshTokenThreshold = new(5, 0, 0, 0);

    private static string DB_STR = string.Empty;

    public static void LoadConfigs(WebApplicationBuilder builder)
    {
        //Выгружаю из файла в константы
        JWT_ISSUER = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("Issuer undefined");
        JWT_AUDIENCE = builder.Configuration["Jwt:Audience"] ?? throw new Exception("audience undefined");
        JWT_KEY = builder.Configuration["Jwt:Key"] ?? throw new Exception("Key undefined");

        REFRESH_HASH_KEY = Encoding.UTF8.GetBytes(builder.Configuration["RefreshToken:Key"] ?? throw new Exception("No RToken hash key"));

        PASSWORD_HASH_KEY = Encoding.UTF8.GetBytes(builder.Configuration["Password:Key"] ?? throw new Exception("No password hash key"));

        DB_STR = builder.Configuration.GetConnectionString("Postgres") ?? throw new Exception("no db connection str");

        SHOP_ID = builder.Configuration["Shop:Id"] ?? throw new Exception("no shop id");
        SHOP_SECRET_KEY = builder.Configuration["Shop:Password"] ?? throw new Exception("no shop password");

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Configs from file loaded");
        Console.ResetColor();
    }

    public static void RegisterServices(WebApplicationBuilder builder)
    {

        AddValidators(builder);

        builder.Services.AddAutoMapper(typeof(ApplicationProfile));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(ConfigureAuth);

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(DB_STR));

        builder.Services.AddStackExchangeRedisCache(x =>
        {
            x.Configuration = builder.Configuration.GetConnectionString("Redis");
            x.InstanceName = "MagazinchikApiCache_";
        });

        AddCustomServices(builder);

        builder.Services.AddQuartz(ConfigureQuartz);
        builder.Services.AddQuartzServer(x => x.WaitForJobsToComplete = true);

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => options.TokenValidationParameters = ConfigureJwtBearer());

        builder.Services.AddCors();
    }

    public static void RegisterEndpoints(WebApplication app)
    {
        new CategoryEndpoints().Define(app);
        new ProductEndpoints().Define(app);
        new AuthEndpoints().Define(app);
        new PhotoEndpoints().Define(app);
        new ReviewEndpoints().Define(app);
        new CartEndpoints().Define(app);
        new OrderEndpoints().Define(app);
        new AddressEndpoints().Define(app);
        new BannerEndpoints().Define(app);
        new FavouriteEndpoints().Define(app);

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Endpoints registered");
        Console.ResetColor();
    }

    public static void Configure(WebApplication app)
    {
        app.UseCors(x =>
            x.WithOrigins("http://localhost:3000")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod());

        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }

    private static void ConfigureQuartz(IServiceCollectionQuartzConfigurator configurator)
    {
        configurator.UseMicrosoftDependencyInjectionJobFactory();
        var jobKey = new JobKey("PaymentChecker");
        configurator.AddJob<PaymentChecker>(options => options.WithIdentity(jobKey));

        configurator.AddTrigger(x => x
        .ForJob(jobKey)
        .WithIdentity("PaymentCheckTrigger", "default")     // идентифицируем триггер с именем и группой
        .StartNow()                            // запуск сразу после начала выполнения 
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever()));
    }

    private static void AddValidators(WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<UserDtoRegistrationValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UserDtoLoginValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CategoryDtoCreateValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoCreateValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoUpdateValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<AddressDtoCreateValidator>();

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Validators added");
        Console.ResetColor();
    }

    private static void AddCustomServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(new Yandex.Checkout.V3.Client(shopId: SHOP_ID, secretKey: SHOP_SECRET_KEY).MakeAsync());
        builder.Services.AddSingleton<IPaymentService, PaymentService>();
        builder.Services.AddScoped<ICacheWrapperService, CacheWrapperService>();
        builder.Services.AddScoped<CommonService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IAddressService, AddressService>();
        builder.Services.AddScoped<IBannerService, BannerService>();
        builder.Services.AddScoped<IFavouriteService, FavouriteService>();


        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Custom Services added");
        Console.ResetColor();
    }

    private static void ConfigureAuth(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions setup)
    {
        // Include 'SecurityScheme' to use JWT Authentication
        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Name = "JWT Authentication",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Кидай jwt token сюда",

            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };

        setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

        setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

    }

    private static TokenValidationParameters ConfigureJwtBearer()
    {
        return new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = JWT_ISSUER,
            ValidAudience = JWT_AUDIENCE,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_KEY))
        };
    }

}