using Microsoft.EntityFrameworkCore;
using PlaylistControl.Models;

namespace PlaylistControl.Data;

public class PlaylistControlDbContext : DbContext
{
    public PlaylistControlDbContext(DbContextOptions<PlaylistControlDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users {get; set;}

    public DbSet<Playlist> Playlists{get; set;}

    public DbSet<Song> Songs {get; set;}

    public DbSet<SongPlaylist> SongPlaylists {get; set;}

    public DbSet<UserPlaylist> UserPlaylists {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasKey(t => t.Id);
        modelBuilder.Entity<Playlist>().HasKey(t => t.Id);
        modelBuilder.Entity<Playlist>().HasOne(u => u.Creator).WithMany(p=>p.MyPlaylists).HasForeignKey(u => u.CreatorId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Song>().HasKey(t => t.Id);
        modelBuilder.Entity<UserPlaylist>().HasKey(k => new {k.UserId, k.PlaylistId});
        modelBuilder.Entity<UserPlaylist>().HasOne(o => o.Playlist).WithMany(p => p.UserPlaylists).HasForeignKey(o => o.PlaylistId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserPlaylist>().HasOne(o => o.User).WithMany(p => p.UserPlaylists).HasForeignKey(o => o.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SongPlaylist>().HasKey(k => new {k.SongId, k.PlaylistId});
        modelBuilder.Entity<SongPlaylist>().HasOne(o => o.Playlist).WithMany(p => p.SongPlaylists).HasForeignKey(o => o.PlaylistId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SongPlaylist>().HasOne(o => o.Song).WithMany(p => p.SongPlaylists).HasForeignKey(o => o.SongId).IsRequired().OnDelete(DeleteBehavior.Restrict);

    }

}