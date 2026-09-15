using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlaylistControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Infrastructure.Persistence.Read.Configurations
{
    /// <summary>
    /// Specifies configurations for User entities
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// Configures User table
        /// </summary>
        /// <param name="builder">Entity builder</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
            builder.Property(u => u.CreatedAt).IsRequired();
        }
    }
}
