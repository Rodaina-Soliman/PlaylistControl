using Microsoft.EntityFrameworkCore;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Infrastructure.Persistence.Read
{
    /// <summary>
    /// Represents the read database
    /// </summary>
    public class PlaylistReadDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistReadDbContext"/> class.
        /// </summary>
        /// <param name="options">Options used by DbContext</param>
        public PlaylistReadDbContext(DbContextOptions<PlaylistReadDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Song> Songs => Set<Song>();
        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<UserPlaylist> UserPlaylists => Set<UserPlaylist>();
        public DbSet<SongPlaylist> SongPlaylists => Set<SongPlaylist>();

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(PlaylistReadDbContext).Assembly,
                t => t.Namespace?.Contains("Persistence.Read.Configurations") == true);

            base.OnModelCreating(modelBuilder);
        }

        /// <inheritdoc/>
        public override int SaveChanges()
            => throw new InvalidOperationException("The read context is read-only.");

        /// <inheritdoc/>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("The read context is read-only.");
    }
}
