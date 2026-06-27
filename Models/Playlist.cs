namespace PlaylistControl.Models;

using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text.Json.Serialization;

public class Playlist
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int CreatorId{get; set;}
    [JsonIgnore]
    public User? Creator {get; set;}
    [JsonIgnore]
    public ICollection<UserPlaylist>? UserPlaylists { get; set; } = new List<UserPlaylist>();
    [JsonIgnore]
    public ICollection<SongPlaylist>? SongPlaylists { get; set; } = new List<SongPlaylist>();

    public Playlist(){}
    public Playlist(string? name, string? description, int creatorId)
    {
        Name = name;
        Description = description;
        CreatorId = creatorId;
    }

}
