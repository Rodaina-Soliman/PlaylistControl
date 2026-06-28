namespace PlaylistControl.Services;

using PlaylistControl.Models;
using PlaylistControl.Data;
using Microsoft.EntityFrameworkCore;

public class PlaylistService
{
    private readonly PlaylistControlDbContext _context;

    public PlaylistService(PlaylistControlDbContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Playlist>> GetAll()
    {
        return await _context.Playlists.AsNoTracking().ToListAsync();
    }

    public async Task<Playlist?> GetPlaylistById(int id)
    {
        return await _context.Playlists.FindAsync(id);
    }

    public async Task<List<Song>> ListSongsInPlaylist(int playlistId)
    {
        return await _context.SongPlaylists.Where(sp => sp.PlaylistId == playlistId).Include(sp => sp.Song).Select(sp => sp.Song).Where(s => s != null).AsNoTracking().ToListAsync() ?? new List<Song>();
    }

    public async Task AddSongToPlaylist(int playlistId, int songId)
    {
        var playlist = await _context.Playlists.FindAsync(playlistId);
        var song = await _context.Songs.FindAsync(songId);

        if (playlist == null || song == null)
            return;

        if (playlist.SongPlaylists == null)
            playlist.SongPlaylists = new List<SongPlaylist>();

        var songPlaylist = new SongPlaylist(songId, playlistId);
        await _context.SongPlaylists.AddAsync(songPlaylist);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveSongFromPlaylist(int playlistId, int songId)
    {
        var playlist = await _context.Playlists.FindAsync(playlistId);
        var songplaylist = await _context.SongPlaylists.FirstOrDefaultAsync(sp => sp.SongId == songId && sp.PlaylistId == playlistId);

        if (playlist == null || songplaylist==null)
            return;

        _context.SongPlaylists.Remove(songplaylist);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePlaylist(int playlistId, Playlist updatedPlaylist)
    {
        var playlist = await _context.Playlists.FindAsync(playlistId);
        if (playlist == null)
            return;

        playlist.Name = updatedPlaylist.Name ?? playlist.Name;
        playlist.Description = updatedPlaylist.Description ?? playlist.Description;
        
        _context.Playlists.Update(playlist);
        await _context.SaveChangesAsync();
    }

}