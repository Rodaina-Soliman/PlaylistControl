using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlaylistControl.Models;

public class UserPlaylist
{
    public int UserId {get; set;}
    public int PlaylistId{get; set;}
    [JsonIgnore]
    public User? User {get; set;}
    [JsonIgnore]
    public Playlist? Playlist {get; set;}
    
    public UserPlaylist(){}
    public UserPlaylist(int userId, int playlistId)
    {
        UserId = userId;
        PlaylistId = playlistId;
    }

}