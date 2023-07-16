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

public static class Starter
{

    private const string SHOP_ID = "229932";
    private const string SECRET_KEY = "test_X6tPeIW1-07vdGAhwRMRfeFCHjtLFT4z7wzlfC_MErA";
    public static byte[] PASSWORD_HASH_KEY { get; private set; } = new byte[1];
    public static byte[] REFRESH_HASH_KEY { get; private set; } = new byte[1];
    public static string JWT_ISSUER { get; private set; } = string.Empty;
    public static string JWT_KEY { get; private set; } = string.Empty;

    public static readonly TimeSpan AccessTokenTime = new(0, 30, 0);
    public static readonly TimeSpan RefreshTokenTime = new(30, 0, 0, 0);

    public static readonly TimeSpan RefreshTokenThreshold = new(5, 0, 0, 0);


    public static void RegisterServices(WebApplicationBuilder builder)
    {

        AddValidators(builder);

        builder.Services.AddAutoMapper(typeof(ApplicationProfile));
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(ConfigureAuth);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
        });


        AddMicroServices(builder);

        builder.Services.AddQuartz(ConfigureQuartz);

        builder.Services.AddQuartzServer(x =>
        {
            x.WaitForJobsToComplete = true;
        });

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => options.TokenValidationParameters = ConfigureJwtBearer(builder));


        builder.Services.AddCors();
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
        .WithSimpleSchedule( x => x.WithIntervalInMinutes(1).RepeatForever()));
    }

    public static void AddValidators(WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<UserDtoRegistrationValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UserDtoLoginValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CathegoryDtoCreateValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoCreateValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoUpdateValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<AddressDtoCreateValidator>();

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Validators added");
        Console.ResetColor();
    }

    public static void AddMicroServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(new Yandex.Checkout.V3.Client(shopId: SHOP_ID, secretKey: SECRET_KEY).MakeAsync());
        builder.Services.AddSingleton<IPaymentService, PaymentService>();
        builder.Services.AddScoped<CommonService>();
        builder.Services.AddScoped<ICathegoryService, CathegoryService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IAddressService, AddressService>();


        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("MicroServices added");
        Console.ResetColor();
    }

    public static void RegisterEndpoints(WebApplication app)
    {
        new CathegoryEndpoints().Define(app);
        new ProductEndpoints().Define(app);
        new AuthEndpoints().Define(app);
        new PhotoEndpoints().Define(app);
        new ReviewEndpoints().Define(app);
        new CartEndpoints().Define(app);
        new OrderEndpoints().Define(app);
        new AddressEndpoints().Define(app);

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Endpoints registered");
        Console.ResetColor();
    }

    public static void LoadConfigs(WebApplication app)
    {
        //Выгружаю из файла в константы
        JWT_ISSUER = app.Configuration["Jwt:Issuer"] ?? throw new Exception("Issuer undefined");
        JWT_KEY = app.Configuration["Jwt:Key"] ?? throw new Exception("Key undefined");
        PASSWORD_HASH_KEY = Encoding.UTF8.GetBytes(app.Configuration["Password:Key"] ?? throw new Exception("No password hash key"));
        REFRESH_HASH_KEY = Encoding.UTF8.GetBytes(app.Configuration["RefreshToken:Key"] ?? throw new Exception("No RToken hash key"));

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Configs from file loaded");
        Console.ResetColor();
    }




    public static void Configure(WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            /*  db.Database.EnsureDeleted();
             db.Database.EnsureCreated(); */
        }

        app.UseCors(builder =>
        {
            builder.WithOrigins("http://localhost:3000");
            builder.AllowCredentials();
            builder.AllowAnyHeader();
        });

        LoadConfigs(app);
    }

    public static void ConfigureAuth(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions setup)
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

    public static TokenValidationParameters ConfigureJwtBearer(WebApplicationBuilder builder)
    {
        return new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new Exception("Отсутствует ключ!")))
        };
    }

}