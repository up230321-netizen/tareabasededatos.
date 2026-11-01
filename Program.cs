using ProductosApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ProductoRepository>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("Productos")
        ?? throw new InvalidOperationException("No se configuró la conexión Productos.");

    return new ProductoRepository(connectionString);
});
builder.Services.AddScoped<IProductoRepository>(serviceProvider =>
    serviceProvider.GetRequiredService<ProductoRepository>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var repository = scope.ServiceProvider.GetRequiredService<ProductoRepository>();
    repository.Inicializar();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
