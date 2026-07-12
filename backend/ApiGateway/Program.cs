var builder = WebApplication.CreateBuilder(args);

// Agregar los servicios de YARP a la inyección de dependencias
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseRouting();

// Mapear el middleware del proxy reverso
app.MapReverseProxy();

app.Run();
