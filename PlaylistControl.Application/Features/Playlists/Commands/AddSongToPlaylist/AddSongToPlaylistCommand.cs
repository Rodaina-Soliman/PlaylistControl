using MediatR;

namespace PlaylistControl.Application.Features.Playlists.Commands.AddSongToPlaylist
{
    /// <summary>
    /// Command for the owner to add a Song to a Playlist
    /// </summary>
    /// <param name="RequesterId">Unique identifier of the acting user</param>
    /// <param name="PlaylistId">Unique identifier of the Playlist</param>
    /// <param name="SongId">Unique identifier of the Song</param>
    public record AddSongToPlaylistCommand(
        Guid RequesterId,
        Guid PlaylistId,
        Guid SongId) : IRequest<bool>;
}