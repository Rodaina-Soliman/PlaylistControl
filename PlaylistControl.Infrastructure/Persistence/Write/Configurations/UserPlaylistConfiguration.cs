using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Infrastructure.Persistence.Write.Configurations
{
    /// <summary>
    /// Specifies configurations for UserPlaylist entities
    /// </summary>
    public class UserPlaylistConfiguration : IEntityTypeConfiguration<UserPlaylist>
    {
        /// <summary>
        /// Configures UserPlaylist table
        /// </summary>
        /// <param name="builder">Entity builder</param>
        public void Configure(EntityTypeBuilder<UserPlaylist> builder)
        {
            builder.ToTable("UserPlaylists");
            builder.HasKey(up => new { up.UserId, up.PlaylistId });
            builder.Property(up => up.AddedAt).IsRequired();
            builder.HasOne(up => up.User)
                .WithMany(u => u.UserPlaylists)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
