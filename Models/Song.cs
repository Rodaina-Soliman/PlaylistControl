using System.ComponentModel.Design.Serialization;
using System.Text.Json.Serialization;
using Microsoft.Identity.Client;

namespace PlaylistControl.Models;

public class Song
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public string? Album { get; set; }
    public int? Year { get; set; }
    public string? Genre { get; set; }
    [JsonIgnore]
    public ICollection<SongPlaylist>? SongPlaylists { get; set; } = new List<SongPlaylist>();

    public Song(){}
    public Song(string title, string artist, string album, int year, string genre)
    {
        Title = title;
        Artist = artist;
        Album = album;
        Year = year;
        Genre = genre;
    }
}