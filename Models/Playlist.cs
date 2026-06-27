namespace PlaylistControl.Models;

using System.Collections.Generic;

public class Playlist
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<Song>? Songs { get; set; } = new List<Song>();

    public Playlist(int id, string? name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

}
