using MediatR;

namespace PlaylistControl.Application.Features.Playlists.Commands.RemovePlaylistFromUser
{
    /// <summary>
    /// Command to remove a Playlist from the requester's own library
    /// </summary>
    /// <param name="RequesterId">Unique identifier of the acting user</param>
    /// <param name="PlaylistId">Unique identifier of the Playlist</param>
    public record RemovePlaylistFromUserCommand(
        Guid RequesterId,
        Guid PlaylistId) : IRequest<bool>;
}