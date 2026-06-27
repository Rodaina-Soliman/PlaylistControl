namespace PlaylistControl.Services;

using PlaylistControl.Models;

public static class UserService
{

    public static List<User> GetAll()
    {
        return new List<User>
        {
            new User { Name = "User 1", Id = 1 },
            new User { Name = "User 2", Id = 2 },
            new User { Name = "User 3", Id = 3 }
        };
    }

    public static User? getUserById(int id)
    {
        return GetAll().FirstOrDefault(u => u.Id == id);
    }

    public static ICollection<Playlist> ListPlaylistsForUser(User user)
    {
        if (user.Playlists != null && user.Playlists.Count > 0)
        {
            foreach (var playlist in user.Playlists)
            {
                Console.WriteLine($"Playlist: {playlist.Name}, Description: {playlist.Description}");
            }
        }
        else
        {
            Console.WriteLine("No playlists for this user.");
        }
        return user.Playlists ?? new List<Playlist>();
    }

    public static void CreatePlaylistForUser(User user, int id, string name, string description)
    {
        var newPlaylist = new Playlist(id, name, description);
        PlaylistService.AddPlaylist(newPlaylist);
        if (user.Playlists == null)
        {
            user.Playlists = new List<Playlist>();
        }
        user.Playlists.Add(newPlaylist);
    }

    public static void RemovePlaylistFromUser(User user, Playlist playlist)
    {
        if (user.Playlists != null)
        {
            user.Playlists.Remove(playlist);
        }
    }
}