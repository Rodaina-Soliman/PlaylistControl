using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetUserPlaylists
{
    /// <summary>
    /// Handler for <see cref="GetUserPlaylistsQuery"/>
    /// </summary>
    public class GetUserPlaylistsQueryHandler : IRequestHandler<GetUserPlaylistsQuery, PagedResult<PlaylistSummaryDto>>
    {
        private readonly IUserReadRepository _userReadRepository;
        private readonly IPlaylistReadRepository _playlistReadRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserPlaylistsQueryHandler"/> class.
        /// </summary>
        /// <param name="userReadRepository">Read-only user repository</param>
        /// <param name="playlistReadRepository">Read-only playlist repository</param>
        public GetUserPlaylistsQueryHandler(
            IUserReadRepository userReadRepository,
            IPlaylistReadRepository playlistReadRepository)
        {
            _userReadRepository = userReadRepository;
            _playlistReadRepository = playlistReadRepository;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<PlaylistSummaryDto>> Handle(GetUserPlaylistsQuery request, CancellationToken cancellationToken)
        {
            if (!await _userReadRepository.ExistsAsync(request.UserId, cancellationToken))
                throw new UserNotFoundException(request.UserId);

            var page = await _playlistReadRepository.GetUserLibraryAsync(
                request.UserId, request.RequesterId, request.Page, request.PageSize, cancellationToken);

            var items = page.Items
                .Select(p => p.ToSummaryDto())
                .ToList();

            return new PagedResult<PlaylistSummaryDto>(
                items, page.Page, page.PageSize, page.TotalCount);
        }
    }
}