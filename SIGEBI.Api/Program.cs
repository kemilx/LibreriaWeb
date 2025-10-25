using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SIGEBI.Api.Filters;
using SIGEBI.IOC;
using SIGEBI.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSIGEBIPersistence(builder.Configuration)
    .AddSIGEBIApplication();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<DomainExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await EnsureDatabaseCreatedAsync(app.Services);

app.Run();

static async Task EnsureDatabaseCreatedAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SIGEBIDbContext>();

    if (!context.Database.IsRelational())
    {
        return;
    }

    try
    {
        await context.Database.EnsureCreatedAsync();
    }
    catch (SqlException ex)
    {
        throw new InvalidOperationException(
            "No se pudo crear o abrir la base de datos SIGEBI. Verifica la cadena de conexión y los permisos del usuario actual.",
            ex);
    }
}
