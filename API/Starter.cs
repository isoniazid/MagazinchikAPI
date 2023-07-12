using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Endpoints;
using MagazinchikAPI.Services;
using MagazinchikAPI.Validators;
using FluentValidation;
using MagazinchikAPI.Services.Photo;
using API.Endpoints;

public static class Starter
{

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

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => options.TokenValidationParameters = ConfigureJwtBearer(builder));


        builder.Services.AddCors();
    }

    public static void AddValidators(WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<UserDtoRegistrationValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UserDtoLoginValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CathegoryDtoCreateValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoCreateValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoUpdateValidator>();

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Validators added");
        Console.ResetColor();
    }

    public static void AddMicroServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<CommonService>();
        builder.Services.AddScoped<ICathegoryService, CathegoryService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<ICartService, CartService>();
        

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