namespace PlaylistControl.Models;

public class User
{
    public string? Name { get; set; }
    public int Id { get; set; }
    public virtual ICollection<Playlist>? Playlists { get; set; } = new List<Playlist>();

}