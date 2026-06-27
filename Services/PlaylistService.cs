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

    public async Task AddPlaylist(Playlist playlist)
    {
        await _context.Playlists.AddAsync(playlist);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Song>> ListSongsInPlaylist(int playlistId)
    {
        var playlist = await GetPlaylistById(playlistId);
        if (playlist == null)
            return new List<Song>();

        return playlist.SongPlaylists?.Select(sp => sp.Song).ToList() ?? new List<Song>();
    }

    public async Task AddSongToPlaylist(int playlistId, int songId)
    {
        var playlist = await _context.Playlists.FindAsync(playlistId);
        var song = await _context.Songs.FindAsync(songId);

        if (playlist == null || song == null)
            return;

        if (playlist.SongPlaylists == null)
            playlist.SongPlaylists = new List<SongPlaylist>();

        var songPlaylist = new SongPlaylist(playlistId, songId, song, playlist);

        playlist.SongPlaylists.Add(songPlaylist);
        await _context.SongPlaylists.AddAsync(songPlaylist);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveSongFromPlaylist(int playlistId, int songId)
    {
        var playlist = await _context.Playlists.FindAsync(playlistId);

        if (playlist == null || playlist.SongPlaylists == null)
            return;

        var songPlaylist = playlist.SongPlaylists.FirstOrDefault(sp => sp.SongId == songId);
        if (songPlaylist != null)
        {
            playlist.SongPlaylists.Remove(songPlaylist);
            _context.SongPlaylists.Remove(songPlaylist);
            await _context.SaveChangesAsync();
        }
    }
}