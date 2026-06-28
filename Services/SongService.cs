namespace PlaylistControl.Services;

using PlaylistControl.Models;
using PlaylistControl.Data;
using Microsoft.EntityFrameworkCore;

public class SongService
{
    private readonly PlaylistControlDbContext _context;

    public SongService(PlaylistControlDbContext context)
    {
        _context = context;
    }

    public async Task<List<Song>> GetAll()
    {
        return await _context.Songs.AsNoTracking().ToListAsync();
    }

    public async Task<Song?> GetSongById(int id)
    {
        return await _context.Songs.FindAsync(id);
    }

    public string GetSongDetails(Song song)
    {
        return $"Title: {song.Title}, Artist: {song.Artist}, Album: {song.Album}, Year: {song.Year}, Genre: {song.Genre}";
    }

    public async Task AddSong(Song song)
    {
        await _context.Songs.AddAsync(song);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSong(int id)
    {
        var song = await _context.Songs
        .Include(s => s.SongPlaylists)
        .FirstOrDefaultAsync(s => s.Id == id);

        if (song == null) 
            return;

        if (song.SongPlaylists != null && song.SongPlaylists.Any())
        {
            _context.SongPlaylists.RemoveRange(song.SongPlaylists);
            await _context.SaveChangesAsync();
        }
        
        _context.Songs.Remove(song);
        await _context.SaveChangesAsync();
        
    }
}