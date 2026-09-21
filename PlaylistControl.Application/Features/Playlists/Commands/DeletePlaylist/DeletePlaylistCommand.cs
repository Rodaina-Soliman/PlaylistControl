using MediatR;

namespace PlaylistControl.Application.Features.Playlists.Commands.DeletePlaylist
{
    /// <summary>
    /// Command for the owner to delete a Playlist
    /// </summary>
    /// <param name="RequesterId">Unique identifier of the acting user</param>
    /// <param name="PlaylistId">Unique identifier of the Playlist</param>
    public record DeletePlaylistCommand(
        Guid RequesterId,
        Guid PlaylistId) : IRequest<bool>;
}