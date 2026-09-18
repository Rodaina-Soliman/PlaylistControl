using MediatR;

namespace PlaylistControl.Application.Features.Playlists.Commands.CreatePlaylist
{
    /// <summary>
    /// Command to create a new Playlist owned by the requester
    /// </summary>
    /// <param name="RequesterId">Unique identifier of the acting user</param>
    /// <param name="Name">Name of the new Playlist</param>
    /// <param name="IsPublic">Whether the new Playlist is public</param>
    public record CreatePlaylistCommand(Guid RequesterId, string Name, bool IsPublic) : IRequest<Guid>;
}