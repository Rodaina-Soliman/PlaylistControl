namespace PlaylistControl.Services;

using PlaylistControl.Models;

public static class SongService
{

    public static List<Song> GetAll()
    {
        return new List<Song>
        {
            new Song { Id = 1, Title = "Song 1", Artist = "Artist 1", Album = "Album 1", Year = 2020, Genre = "Pop" },
            new Song { Id = 2, Title = "Song 2", Artist = "Artist 2", Album = "Album 2", Year = 2019, Genre = "Rock" },
            new Song { Id = 3, Title = "Song 3", Artist = "Artist 3", Album = "Album 3", Year = 2021, Genre = "Hip-Hop" }
        };
    }

    public static string GetSongDetails(Song song)
    {
        return $"Title: {song.Title}, Artist: {song.Artist}, Album: {song.Album}, Year: {song.Year}, Genre: {song.Genre}";
    }

    public static Song? GetSongById(int id)
    {
        return GetAll().FirstOrDefault(s => s.Id == id);
    }
    
}