namespace PlaylistControl.Models;
using System.Text.Json.Serialization;

public class User
{
    public string? Name { get; set; }
    public int Id { get; set; }
    [JsonIgnore]
    public ICollection<Playlist>? MyPlaylists { get; set; } = new List<Playlist>();
    [JsonIgnore]
    public ICollection<UserPlaylist>? UserPlaylists { get; set; } = new List<UserPlaylist>();

    public User(){}
    public User(string name)
    {
        Name = name;
    }

}