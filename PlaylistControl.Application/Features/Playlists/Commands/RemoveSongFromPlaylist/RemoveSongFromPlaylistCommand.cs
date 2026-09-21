using MediatR;

namespace PlaylistControl.Application.Features.Playlists.Commands.RemoveSongFromPlaylist
{
    /// <summary>
    /// Command for the owner to remove a Song from a Playlist
    /// </summary>
    /// <param name="RequesterId">Unique identifier of the acting user</param>
    /// <param name="PlaylistId">Unique identifier of the Playlist</param>
    /// <param name="SongId">Unique identifier of the Song</param>
    public record RemoveSongFromPlaylistCommand(
        Guid RequesterId,
        Guid PlaylistId,
        Guid SongId) : IRequest<bool>;
}