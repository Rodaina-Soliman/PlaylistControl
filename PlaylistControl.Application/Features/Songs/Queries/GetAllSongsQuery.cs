using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Models;

namespace PlaylistControl.Application.Features.Songs.Queries.GetAllSongs
{
    /// <summary>
    /// Query for a page of the static Song catalog
    /// </summary>
    /// <param name="Page">1-based page number</param>
    /// <param name="PageSize">Number of items per page</param>
    public record GetAllSongsQuery(
        int Page = 1,
        int PageSize = 1) : IRequest<PagedResult<SongDto>>;
}