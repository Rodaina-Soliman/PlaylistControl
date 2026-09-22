using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Infrastructure.Persistence.Read;
using PlaylistControl.Infrastructure.Persistence.Read.Repositories;
using PlaylistControl.Infrastructure.Persistence.Seed;
using PlaylistControl.Infrastructure.Persistence.Write;
using PlaylistControl.Infrastructure.Persistence.Write.Repositories;

namespace PlaylistControl.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        public CustomWebApplicationFactory()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        public static readonly Guid AliceId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid BobId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid CarolId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        public static readonly Guid BohemianRhapsodyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        public static readonly Guid HotelCaliforniaId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        public static readonly Guid StairwayToHeavenId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        public static readonly Guid ImagineId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        public static readonly Guid SmellsLikeTeenSpiritId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                var efDescriptors = services
                    .Where(d =>
                        (d.ServiceType.FullName?.StartsWith("Microsoft.EntityFrameworkCore") ?? false)
                        || (d.ServiceType.FullName?.StartsWith("Microsoft.Data.SqlClient") ?? false)
                        || (d.ImplementationType?.FullName?.Contains("SqlServer") ?? false))
                    .ToList();

                foreach (var descriptor in efDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<PlaylistWriteDbContext>(o => o.UseSqlite(_connection));
                services.AddDbContext<PlaylistReadDbContext>(o => o.UseSqlite(_connection));

                services.RemoveAll<IPlaylistWriteRepository>();
                services.RemoveAll<IPlaylistReadRepository>();
                services.RemoveAll<IUserReadRepository>();
                services.RemoveAll<ISongReadRepository>();

                services.AddScoped<IPlaylistWriteRepository, PlaylistWriteRepository>();
                services.AddScoped<IPlaylistReadRepository, PlaylistReadRepository>();
                services.AddScoped<IUserReadRepository, UserReadRepository>();
                services.AddScoped<ISongReadRepository, SongReadRepository>();
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();
            var writeContext = scope.ServiceProvider.GetRequiredService<PlaylistWriteDbContext>();
            var readContext = scope.ServiceProvider.GetRequiredService<PlaylistReadDbContext>();

            writeContext.Database.EnsureCreated();
            readContext.Database.EnsureCreated();
            DatabaseSeeder.SeedAsync(writeContext).GetAwaiter().GetResult();

            return host;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection.Dispose();
            }
        }
    }
}