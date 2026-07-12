using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NeoLibroAPI.Interfaces;
using NeoLibroAPI.Data;
using NeoLibroAPI.Business;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
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
        logger.LogInformation("📥 [DB-READ] Conectando a replica de lectura...");
        logger.LogInformation("==================================================");
    }
    else
    {
        connStr = config.GetConnectionString("WriteConnection") 
            ?? config.GetConnectionString("cnnNeoLibroDB")!;
        logger.LogInformation("==================================================");
        logger.LogInformation("📤 [DB-WRITE] Ejecutando transaccion en BD principal...");
        logger.LogInformation("==================================================");
    }
    return connStr;
});

// Registrar servicios NUEVOS con interfaces (MÓDULO USUARIOS)
builder.Services.AddScoped<IUsuarioRepository>(provider => new UsuarioRepository(provider.GetRequiredService<string>()));
builder.Services.AddScoped<IUsuarioBusiness, UsuarioBusiness>();

// Configurar clientes HTTP para repositorios remotos (MÓDULO LIBROS Y EJEMPLARES)
var catalogoUrl = builder.Configuration["Services:CatalogoUrl"] ?? "http://localhost:5201/";
builder.Services.AddHttpClient<ILibroRepository, HttpLibroRepository>(client =>
{
    client.BaseAddress = new Uri(catalogoUrl);
});
builder.Services.AddHttpClient<IEjemplarRepository, HttpEjemplarRepository>(client =>
{
    client.BaseAddress = new Uri(catalogoUrl);
});
builder.Services.AddScoped<ILibroBusiness, LibroBusiness>();
builder.Services.AddScoped<IEjemplarBusiness, EjemplarBusiness>();

// Registrar servicios NUEVOS con interfaces (MÓDULO PRÉSTAMOS)
builder.Services.AddScoped<IPrestamoRepository>(provider => 
    new PrestamoRepository(provider.GetRequiredService<string>(), provider.GetRequiredService<IEjemplarRepository>()));
builder.Services.AddScoped<IPrestamoBusiness, PrestamoBusiness>();

// Registrar servicios NUEVOS con interfaces (MÓDULO MULTAS)
builder.Services.AddScoped<IMultaRepository>(provider => new MultaRepository(provider.GetRequiredService<string>()));
builder.Services.AddScoped<IMultaBusiness, MultaBusiness>();

// Registrar servicios NUEVOS con interfaces (MÓDULO RESERVAS)
builder.Services.AddDbContext<ApplicationDbContext>((provider, options) =>
    options.UseSqlServer(provider.GetRequiredService<string>()));

builder.Services.AddScoped<IReservaRepository>(provider => 
    new ReservaRepository(provider.GetRequiredService<ApplicationDbContext>(), provider.GetRequiredService<string>()));
builder.Services.AddScoped<IReservaBusiness, ReservaBusiness>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();



builder.Services.AddAuthentication("MiCookieAuth")
    .AddCookie("MiCookieAuth", options =>
    {
        options.Cookie.Name = "NeoLibro.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Para DEV (HTTP)
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath = "/api/Usuarios/login";
        options.AccessDeniedPath = "/api/Usuarios/acceso-denegado";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;

        // Clave: NO redirecciones para /api
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }
                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
                ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
    options.InstanceName = "Biblioteca_";
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseCors("FrontendDev");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.Run();