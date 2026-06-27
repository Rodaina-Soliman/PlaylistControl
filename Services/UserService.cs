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
        User? user = await _context.Users.FindAsync(id);
        if (user==null)
            return;
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
        return await _context.UserPlaylists.Where(up => up.UserId == userId).Include(up => up.Playlist).Select(up => up.Playlist).AsNoTracking().ToListAsync() ?? new List<Playlist>();
    }

    public async Task CreatePlaylistForUser(User user, Playlist playlist)
    {
        playlist.CreatorId = user.Id;
        playlist.Creator = user;
        
        await _context.Playlists.AddAsync(playlist);
        await _context.SaveChangesAsync();
        
        var userPlaylist = new UserPlaylist(user.Id, playlist.Id, user, playlist);
        
        await _context.UserPlaylists.AddAsync(userPlaylist);
        await _context.SaveChangesAsync();
    }

    public async Task RemovePlaylistFromUser(int userId, int playlistId)
    {
        var userPlaylist = await _context.UserPlaylists.FirstOrDefaultAsync(up => up.UserId == userId && up.PlaylistId == playlistId);
        
        if (userPlaylist != null)
        {
            _context.UserPlaylists.Remove(userPlaylist);
            await _context.SaveChangesAsync();
        }
        
        var playlist = await _context.Playlists.FindAsync(playlistId);
        if (playlist != null && playlist.CreatorId==userId) //only delete the playlist if the user created it
        {
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
        }
    }
}