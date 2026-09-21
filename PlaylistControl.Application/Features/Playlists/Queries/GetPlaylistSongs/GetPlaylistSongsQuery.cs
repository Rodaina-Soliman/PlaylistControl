using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Models;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetPlaylistSongs
{
    /// <summary>
    /// Query for a page of Songs in a Playlist
    /// </summary>
    /// <param name="RequesterId">Unique identifier of the acting user</param>
    /// <param name="PlaylistId">Unique identifier of the Playlist</param>
    /// <param name="Page">1-based page number</param>
    /// <param name="PageSize">Number of items per page</param>
    public record GetPlaylistSongsQuery(
        Guid RequesterId,
        Guid PlaylistId,
        int Page = 1,
        int PageSize = 1) : IRequest<PagedResult<SongDto>>;
}