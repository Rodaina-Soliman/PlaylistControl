using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetPlaylistSongs
{
    /// <summary>
    /// Handler for <see cref="GetPlaylistSongsQuery"/>
    /// </summary>
    public class GetPlaylistSongsQueryHandler : IRequestHandler<GetPlaylistSongsQuery, PagedResult<SongDto>>
    {
        private readonly IPlaylistReadRepository _playlistReadRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetPlaylistSongsQueryHandler"/> class.
        /// </summary>
        /// <param name="playlistReadRepository">Read-only playlist repository</param>
        public GetPlaylistSongsQueryHandler(IPlaylistReadRepository playlistReadRepository)
        {
            _playlistReadRepository = playlistReadRepository;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<SongDto>> Handle(GetPlaylistSongsQuery request, CancellationToken cancellationToken)
        {
            var page = await _playlistReadRepository.GetPlaylistWithPagedSongsAsync(
                request.PlaylistId, request.Page, request.PageSize, cancellationToken);

            if (page.Playlist is null)
                throw new PlaylistNotFoundException(request.PlaylistId);

            if (!page.Playlist.IsPublic
                && page.Playlist.OwnerId != request.RequesterId
                && !await _playlistReadRepository.IsInUserLibraryAsync(request.RequesterId, request.PlaylistId, cancellationToken))
            {
                throw new PrivatePlaylistAccessException(request.PlaylistId);
            }

            var items = page.Songs.Items
                .Select(s => s.ToSongDto())
                .ToList();

            return new PagedResult<SongDto>(
                items, page.Songs.Page, page.Songs.PageSize, page.Songs.TotalCount);
        }
    }
}