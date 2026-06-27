namespace PlaylistControl.Services;

using PlaylistControl.Models;

public static class PlaylistService
{

    static ICollection<Playlist> playlists = new List<Playlist>
    {
        new Playlist(1, "Chill Vibes", "Relaxing tunes for a calm evening"),
        new Playlist(2, "Workout Hits", "High-energy tracks to keep you moving"),
        new Playlist(3, "Classic Rock", "Timeless rock anthems from the 70s and 80s")
    };

    public static ICollection<Playlist> GetAll()
    {
        return playlists;
    }

    public static Playlist? GetPlaylistById(int id)
    {
        return GetAll().FirstOrDefault(p => p.Id == id);
    }

    public static void AddPlaylist(Playlist playlist)
    {
        playlists.Add(playlist);
    }

    public static void ListSongsInPlaylist(Playlist playlist)
    {
        if (playlist.Songs != null && playlist.Songs.Count > 0)
        {
            foreach (var song in playlist.Songs)
            {
               Console.WriteLine(SongService.GetSongDetails(song));
            }
        }
        else
        {
            Console.WriteLine("No songs in the playlist.");
        }
    }

    public static void AddSongToPlaylist(Playlist playlist, Song song)
    {
        if (playlist.Songs == null)
        {
            playlist.Songs = new List<Song>();
        }
        playlist.Songs.Add(song);
    }

    public static void RemoveSongFromPlaylist(Playlist playlist, Song song)
    {
        if (playlist.Songs != null)
        {
            playlist.Songs.Remove(song);
        }
    }
    
}