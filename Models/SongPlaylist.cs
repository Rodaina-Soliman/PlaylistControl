using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PlaylistControl.Models;

public class SongPlaylist
{
    public int SongId {get; set;}
    public int PlaylistId{get; set;}
    [JsonIgnore]
    public Song? Song {get; set;}
    [JsonIgnore]
    public Playlist? Playlist {get; set;}
    
    public SongPlaylist(){}
    public SongPlaylist(int songId, int playlistId, Song song, Playlist playlist)
    {
        SongId = songId;
        PlaylistId = playlistId;
        Song = song;
        Playlist = playlist;
    }
    
}