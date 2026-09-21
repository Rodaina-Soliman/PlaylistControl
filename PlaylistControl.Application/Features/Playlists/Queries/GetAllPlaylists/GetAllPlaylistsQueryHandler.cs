using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetAllPlaylists
{
    /// <summary>
    /// Handler for <see cref="GetAllPlaylistsQuery"/>
    /// </summary>
    public class GetAllPlaylistsQueryHandler : IRequestHandler<GetAllPlaylistsQuery, PagedResult<PlaylistSummaryDto>>
    {
        private readonly IPlaylistReadRepository _playlistReadRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllPlaylistsQueryHandler"/> class.
        /// </summary>
        /// <param name="playlistReadRepository">Read-only playlist repository</param>
        public GetAllPlaylistsQueryHandler(IPlaylistReadRepository playlistReadRepository)
        {
            _playlistReadRepository = playlistReadRepository;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PlaylistSummaryDto>> Handle(GetAllPlaylistsQuery request, CancellationToken cancellationToken)
        {
            var page = await _playlistReadRepository.GetPublicAndOwnedAsync(
                request.RequesterId, request.Page, request.PageSize, cancellationToken);

            var items = page.Items
                .Select(p => p.ToSummaryDto())
                .ToList();

            return new PagedResult<PlaylistSummaryDto>(
                items, page.Page, page.PageSize, page.TotalCount);
        }
    }
}