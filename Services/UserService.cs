namespace PlaylistControl.Services;

using Microsoft.EntityFrameworkCore;
using PlaylistControl.Data;
using PlaylistControl.Models;
public class UserService
{
    private readonly PlaylistControlDbContext _context;

    public UserService(PlaylistControlDbContext context)
    {
        _context = context;
    }

    public async Task Add(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {

        var user = await _context.Users
        .Include(u => u.UserPlaylists)
        .Include(u => u.MyPlaylists)
        .ThenInclude(p => p.UserPlaylists)
        .Include(u => u.MyPlaylists)
        .ThenInclude(p => p.SongPlaylists)
        .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return;


        if (user.UserPlaylists.Any())
        {
            _context.UserPlaylists.RemoveRange(user.UserPlaylists);
            await _context.SaveChangesAsync();
        }

        foreach (var playlist in user.MyPlaylists.ToList())
        {

            var userPlaylistsForPlaylist = await _context.UserPlaylists.Where(up => up.PlaylistId == playlist.Id).ToListAsync();

            if (userPlaylistsForPlaylist.Any())
            {
                _context.UserPlaylists.RemoveRange(userPlaylistsForPlaylist);
                await _context.SaveChangesAsync();
            }

            var songPlaylistsForPlaylist = await _context.SongPlaylists.Where(sp => sp.PlaylistId == playlist.Id).ToListAsync();

            if (songPlaylistsForPlaylist.Any())
            {
                _context.SongPlaylists.RemoveRange(songPlaylistsForPlaylist);
                await _context.SaveChangesAsync();
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetAll()
    {
        return await _context.Users.AsNoTracking().ToListAsync();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<ICollection<Playlist>> ListPlaylistsForUser(int userId)
    {
        return await _context.UserPlaylists
            .Where(up => up.UserId == userId)
            .Include(up => up.Playlist)
            .Select(up => up.Playlist)
            .AsNoTracking()
            .ToListAsync() ?? new List<Playlist>();
    }

    public async Task<bool> PlaylistBelongsToUser(int playlistId, int userId)
    {
        return await _context.UserPlaylists
            .AnyAsync(up => up.UserId == userId && up.PlaylistId == playlistId);
    }

    public async Task CreatePlaylistForUser(User user, Playlist playlist)
    {
        var newPlaylist = new Playlist(playlist.Name, playlist.Description, user.Id);
        await _context.Playlists.AddAsync(newPlaylist);
        await _context.SaveChangesAsync();

        var userPlaylist = new UserPlaylist(user.Id, newPlaylist.Id);
        await _context.UserPlaylists.AddAsync(userPlaylist);
        await _context.SaveChangesAsync();
    }

    public async Task AddPlaylistForUser(int userId, int playlistId)
    {
        var exists = await _context.UserPlaylists
            .AnyAsync(up => up.UserId == userId && up.PlaylistId == playlistId);

        if (!exists)
        {
            var userPlaylist = new UserPlaylist(userId, playlistId);
            await _context.UserPlaylists.AddAsync(userPlaylist);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemovePlaylistFromUser(int userId, int playlistId)
    {
        var userPlaylist = await _context.UserPlaylists
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PlaylistId == playlistId);

        if (userPlaylist == null)
            return;

        var playlist = await _context.Playlists
            .Include(p => p.UserPlaylists)
            .Include(p => p.SongPlaylists)
            .FirstOrDefaultAsync(p => p.Id == playlistId);

        if (playlist == null)
            return;

        var isCreator = playlist.CreatorId == userId;

        _context.UserPlaylists.Remove(userPlaylist);
        await _context.SaveChangesAsync();

        if (isCreator)
        {
            var otherUsersHaveIt = await _context.UserPlaylists
                .AnyAsync(up => up.PlaylistId == playlistId);

            if (!otherUsersHaveIt)
            {
                if (playlist.SongPlaylists.Any())
                {
                    _context.SongPlaylists.RemoveRange(playlist.SongPlaylists);
                }

                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task UpdateUser(int userId, User updatedUser)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return;

        user.Name = updatedUser.Name ?? user.Name;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

}