using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Infrastructure.Persistence.Read;
using PlaylistControl.Infrastructure.Persistence.Read.Repositories;
using PlaylistControl.Infrastructure.Persistence.Write;
using PlaylistControl.Infrastructure.Persistence.Write.Repositories;

namespace PlaylistControl.Infrastructure
{
    /// <summary>
    /// Extension method for registering Infrastructure layer services
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers the read and write DbContexts along with all repository implementations
        /// </summary>
        /// <param name="services">The service collection to add registrations to</param>
        /// <param name="configuration">Application configuration containing the connection string</param>
        /// <returns>The same service collection, for chaining</returns>
        /// <exception cref="InvalidOperationException">Thrown when the "DefaultConnection" connection string is missing</exception>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<PlaylistWriteDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDbContext<PlaylistReadDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IPlaylistWriteRepository, PlaylistWriteRepository>();

            services.AddScoped<IPlaylistReadRepository, PlaylistReadRepository>();
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<ISongReadRepository, SongReadRepository>();

            return services;
        }
    }
}