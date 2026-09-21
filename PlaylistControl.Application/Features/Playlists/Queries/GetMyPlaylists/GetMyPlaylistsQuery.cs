using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Models;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetMyPlaylists
{
    /// <summary>
    /// Query for a page of Playlists owned by the requester
    /// </summary>
    /// <param name="RequesterId">Unique identifier of the acting user</param>
    /// <param name="Page">1-based page number</param>
    /// <param name="PageSize">Number of items per page</param>
    public record GetMyPlaylistsQuery(
        Guid RequesterId,
        int Page = 1,
        int PageSize = 1) : IRequest<PagedResult<PlaylistSummaryDto>>;
}