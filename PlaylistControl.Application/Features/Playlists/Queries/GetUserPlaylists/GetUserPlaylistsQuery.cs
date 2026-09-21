using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Models;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetUserPlaylists
{
    /// <summary>
    /// Query for a page of Playlists in another User's library
    /// </summary>
    /// <param name="RequesterId">Unique identifier of the acting user</param>
    /// <param name="UserId">Unique identifier of the User whose library is queried</param>
    /// <param name="Page">1-based page number</param>
    /// <param name="PageSize">Number of items per page</param>
    public record GetUserPlaylistsQuery(
        Guid RequesterId,
        Guid UserId,
        int Page = 1,
        int PageSize = 1) : IRequest<PagedResult<PlaylistSummaryDto>>;
}