using Microsoft.AspNetCore.Authentication.Cookies;
using NeoLibroAPI.Interfaces;
using NeoLibroAPI.Data;
using NeoLibroAPI.Business;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configurar cadena de conexión dinámica y registrar HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<string>(provider =>
{
    var httpContext = provider.GetService<IHttpContextAccessor>()?.HttpContext;
    var config = provider.GetRequiredService<IConfiguration>();
    var logger = provider.GetRequiredService<ILogger<Program>>();

    string connStr;
    if (httpContext != null && httpContext.Request.Method == HttpMethods.Get)
    {
        connStr = config.GetConnectionString("ReadOnlyConnection") 
            ?? config.GetConnectionString("cnnNeoLibroDB")!;
        logger.LogInformation("==================================================");
        logger.LogInformation("📥 [DB-READ - CATALOGO] Conectando a replica...");
        logger.LogInformation("==================================================");
    }
    else
    {
        connStr = config.GetConnectionString("WriteConnection") 
            ?? config.GetConnectionString("cnnNeoLibroDB")!;
        logger.LogInformation("==================================================");
        logger.LogInformation("📤 [DB-WRITE - CATALOGO] Conectando a principal...");
        logger.LogInformation("==================================================");
    }
    return connStr;
});

// Configurar Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("Redis:Configuration") ?? "localhost:6379";
});

// Registrar servicios con interfaces (MÓDULO AUTORES)
builder.Services.AddScoped<IAutorRepository>(provider => new AutorRepository(provider.GetRequiredService<string>()));
builder.Services.AddScoped<IAutorBusiness, AutorBusiness>();

// Registrar servicios con interfaces (MÓDULO CATEGORÍAS)
builder.Services.AddScoped<ICategoriaRepository>(provider => new CategoriaRepository(provider.GetRequiredService<string>()));
builder.Services.AddScoped<ICategoriaBusiness, CategoriaBusiness>();

// Registrar servicios con interfaces (MÓDULO LIBROS)
builder.Services.AddScoped<ILibroRepository>(provider => new LibroRepository(provider.GetRequiredService<string>()));
builder.Services.AddScoped<ILibroBusiness, LibroBusiness>();

// Registrar servicios con interfaces (MÓDULO EJEMPLARES)
builder.Services.AddScoped<IEjemplarRepository>(provider => new EjemplarRepository(provider.GetRequiredService<string>()));
builder.Services.AddScoped<IEjemplarBusiness, EjemplarBusiness>();

// Configurar autenticación por Cookie igual al Monolito para que se descifre correctamente
builder.Services.AddAuthentication("MiCookieAuth")
    .AddCookie("MiCookieAuth", options =>
    {
        options.Cookie.Name = "NeoLibro.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
