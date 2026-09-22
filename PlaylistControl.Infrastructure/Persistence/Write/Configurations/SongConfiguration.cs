using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Infrastructure.Persistence.Write.Configurations
{
    /// <summary>
    /// Specifies configurations for Song entities
    /// </summary>
    public class SongConfiguration : IEntityTypeConfiguration<Song>
    {
        /// <summary>
        /// Configures Song table
        /// </summary>
        /// <param name="builder">Entity builder</param>
        public void Configure(EntityTypeBuilder<Song> builder)
        {
            builder.ToTable("Songs");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
            builder.Property(s => s.Artist).IsRequired().HasMaxLength(200);
            builder.Property(s => s.DurationSeconds).IsRequired();
        }
    }
}
