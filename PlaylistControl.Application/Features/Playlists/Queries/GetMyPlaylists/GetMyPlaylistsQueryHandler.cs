using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetMyPlaylists
{
    /// <summary>
    /// Handler for <see cref="GetMyPlaylistsQuery"/>
    /// </summary>
    public class GetMyPlaylistsQueryHandler : IRequestHandler<GetMyPlaylistsQuery, PagedResult<PlaylistSummaryDto>>
    {
        private readonly IPlaylistReadRepository _playlistReadRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMyPlaylistsQueryHandler"/> class.
        /// </summary>
        /// <param name="playlistReadRepository">Read-only playlist repository</param>
        public GetMyPlaylistsQueryHandler(IPlaylistReadRepository playlistReadRepository)
        {
            _playlistReadRepository = playlistReadRepository;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PlaylistSummaryDto>> Handle(GetMyPlaylistsQuery request, CancellationToken cancellationToken)
        {
            var page = await _playlistReadRepository.GetByOwnerAsync(
                request.RequesterId, request.Page, request.PageSize, cancellationToken);

            var items = page.Items
                .Select(p => p.ToSummaryDto())
                .ToList();

            return new PagedResult<PlaylistSummaryDto>(
                items, page.Page, page.PageSize, page.TotalCount);
        }
    }
}