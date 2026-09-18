using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Infrastructure.Persistence.Read.Configurations
{
    /// <summary>
    /// Specifies configurations for SongPlaylist entities
    /// </summary>
    public class SongPlaylistConfiguration : IEntityTypeConfiguration<SongPlaylist>
    {
        /// <summary>
        /// Configures SongPlaylist table
        /// </summary>
        /// <param name="builder">Entity builder</param>
        public void Configure(EntityTypeBuilder<SongPlaylist> builder)
        {
            builder.ToTable("SongPlaylists");
            builder.HasKey(sp => new { sp.SongId, sp.PlaylistId });
            builder.Property(sp => sp.AddedAt).IsRequired();
            builder.HasOne(sp => sp.Song)
                .WithMany(s => s.SongPlaylists)
                .HasForeignKey(sp => sp.SongId);
            builder.HasOne(sp => sp.Playlist)
                .WithMany(p => p.SongPlaylists)
                .HasForeignKey(sp => sp.PlaylistId);
        }
    }
}
