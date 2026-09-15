using Microsoft.EntityFrameworkCore;
using PlaylistControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Infrastructure.Persistence.Write
{
    /// <summary>
    /// Represents the write database
    /// </summary>
    public class PlaylistWriteDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistWriteDbContext"/> class.
        /// </summary>
        /// <param name="options">Options used by DbContext</param>
        public PlaylistWriteDbContext(DbContextOptions<PlaylistWriteDbContext> options) : base(options) { }

        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<UserPlaylist> UserPlaylists => Set<UserPlaylist>();
        public DbSet<SongPlaylist> SongPlaylists => Set<SongPlaylist>();

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(PlaylistWriteDbContext).Assembly,
                t => t.Namespace?.Contains("Persistence.Write.Configurations") == true);

            base.OnModelCreating(modelBuilder);
        }
    }
}
