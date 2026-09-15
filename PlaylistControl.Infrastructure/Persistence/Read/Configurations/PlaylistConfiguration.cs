using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaylistControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Infrastructure.Persistence.Read.Configurations
{
    /// <summary>
    /// Specifies configurations for Playlist entities
    /// </summary>
    public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
    {
        /// <summary>
        /// Configures Playlist table
        /// </summary>
        /// <param name="builder">Entity builder</param>
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder.ToTable("Playlists");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.IsPublic).IsRequired();
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.HasOne(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(p => p.UserPlaylists)
                .WithOne(up => up.Playlist)
                .HasForeignKey(up => up.PlaylistId);
            builder.HasMany(p => p.SongPlaylists)
                .WithOne(sp => sp.Playlist)
                .HasForeignKey(sp => sp.PlaylistId);
        }
    }
}
