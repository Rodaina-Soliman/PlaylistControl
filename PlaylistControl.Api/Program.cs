using PlaylistControl.Infrastructure;
using PlaylistControl.Infrastructure.Persistence.Seed;
using PlaylistControl.Infrastructure.Persistence.Write;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var writeContext = scope.ServiceProvider.GetRequiredService<PlaylistWriteDbContext>();
    await DatabaseSeeder.SeedAsync(writeContext);
}

// Configure the HTTP request pipeline.

app.Run();
