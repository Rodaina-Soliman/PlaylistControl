using MediatR;

namespace PlaylistControl.Application.Features.Playlists.Commands.UpdatePlaylist
{
    /// <summary>
    /// Command to rename and/or change the privacy of a Playlist
    /// </summary>
    /// <param name="RequesterId">Unique identifier of the acting user</param>
    /// <param name="PlaylistId">Unique identifier of the Playlist</param>
    /// <param name="Name">New name, or null to leave unchanged</param>
    /// <param name="IsPublic">New privacy, or null to leave unchanged</param>
    public record UpdatePlaylistCommand(
        Guid RequesterId,
        Guid PlaylistId,
        string? Name,
        bool? IsPublic) : IRequest<bool>;
}