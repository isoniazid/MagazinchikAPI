using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Endpoints;
using MagazinchikAPI.Services;
using MagazinchikAPI.DTO;
using MagazinchikAPI.Validators;
using FluentValidation;

public static class Starter
{

    public static readonly byte[] passwordHashKey = Encoding.UTF8.GetBytes("Здарова Вовчик, а ты че исходники смотришь мои?");

    public static readonly byte[] RefreshHashKey = Encoding.UTF8.GetBytes("Это ключ для хеширования рефреш токенов. Кста Вовыч здарова как жизнь?");
    public static readonly TimeSpan AccessTokenTime = new(0, 30, 0);
    public static readonly TimeSpan RefreshTokenTime = new(30,0,0,0);

    public static readonly TimeSpan RefreshTokenThreshold = new(5, 0, 0, 0);
    
    private static string jwt_issuer_ = string.Empty;
    public static string JWT_ISSUER {get => jwt_issuer_;} 
    private static string jwt_key_ = string.Empty;
    public static string JWT_KEY {get => jwt_key_;}

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
    }

    public static void AddMicroServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICathegoryService, CathegoryService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
    }

        public static void RegisterEndpoints(WebApplication app)
    {
        new CathegoryEndpoints().Define(app);
        new ProductEndpoints().Define(app);
        new AuthEndpoints().Define(app);
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
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        app.UseCors(builder =>
        {
            builder.WithOrigins("http://localhost:3000");
            builder.AllowCredentials();
            builder.AllowAnyHeader();
        });

        //Выгружаю из файла в константы
        jwt_issuer_ = app.Configuration["Jwt:Issuer"] ?? throw new Exception("Issuer undefined");
        jwt_key_ = app.Configuration["Jwt:Key"] ?? throw new Exception("Key undefined");
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

}