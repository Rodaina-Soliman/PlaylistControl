using PlaylistControl.Api.Middleware;
using PlaylistControl.Application;
using PlaylistControl.Infrastructure;
using PlaylistControl.Infrastructure.Persistence.Seed;
using PlaylistControl.Infrastructure.Persistence.Write;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var writeContext = scope.ServiceProvider.GetRequiredService<PlaylistWriteDbContext>();
    await DatabaseSeeder.SeedAsync(writeContext);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();

public partial class Program { }