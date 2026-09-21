using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Application.Common.Models
{
    /// <summary>
    /// Carries playlist metadata (for access checks) together with a page of its Songs
    /// </summary>
    /// <param name="Playlist">Playlist metadata, or null if the playlist was not found</param>
    /// <param name="Songs">Page of Songs belonging to the playlist</param>
    public record PlaylistWithSongsPage(
        Playlist? Playlist,
        PagedResult<Song> Songs);
}