using MediatR;

namespace PlaylistControl.Application.Features.Playlists.Commands.AddPlaylistToUser
{
    /// <summary>
    /// Command to add a Playlist to a target User's library
    /// </summary>
    /// <param name="RequesterId">Unique identifier of the acting user</param>
    /// <param name="TargetUserId">Unique identifier of the User whose library receives the Playlist</param>
    /// <param name="PlaylistId">Unique identifier of the Playlist</param>
    public record AddPlaylistToUserCommand(
        Guid RequesterId,
        Guid TargetUserId,
        Guid PlaylistId) : IRequest<bool>;
}