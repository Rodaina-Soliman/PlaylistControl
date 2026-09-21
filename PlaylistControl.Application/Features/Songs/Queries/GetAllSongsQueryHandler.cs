using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;

namespace PlaylistControl.Application.Features.Songs.Queries.GetAllSongs
{
    /// <summary>
    /// Handler for <see cref="GetAllSongsQuery"/>
    /// </summary>
    public class GetAllSongsQueryHandler : IRequestHandler<GetAllSongsQuery, PagedResult<SongDto>>
    {
        private readonly ISongReadRepository _songReadRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllSongsQueryHandler"/> class.
        /// </summary>
        /// <param name="songReadRepository">Read-only song repository</param>
        public GetAllSongsQueryHandler(ISongReadRepository songReadRepository)
        {
            _songReadRepository = songReadRepository;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<SongDto>> Handle(GetAllSongsQuery request, CancellationToken cancellationToken)
        {
            var page = await _songReadRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);

            var items = page.Items
                .Select(s => s.ToSongDto())
                .ToList();

            return new PagedResult<SongDto>(
                items, page.Page, page.PageSize, page.TotalCount);
        }
    }
}