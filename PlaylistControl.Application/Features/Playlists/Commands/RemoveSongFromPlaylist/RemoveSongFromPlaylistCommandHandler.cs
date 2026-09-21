using MediatR;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.Application.Features.Playlists.Commands.RemoveSongFromPlaylist
{
    /// <summary>
    /// Handler for <see cref="RemoveSongFromPlaylistCommand"/>
    /// </summary>
    public class RemoveSongFromPlaylistCommandHandler : IRequestHandler<RemoveSongFromPlaylistCommand, bool>
    {
        private readonly IPlaylistWriteRepository _playlistWriteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveSongFromPlaylistCommandHandler"/> class.
        /// </summary>
        /// <param name="playlistWriteRepository">Write-side playlist repository</param>
        public RemoveSongFromPlaylistCommandHandler(IPlaylistWriteRepository playlistWriteRepository)
        {
            _playlistWriteRepository = playlistWriteRepository;
        }

        /// <inheritdoc/>
        public async Task<bool> Handle(RemoveSongFromPlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _playlistWriteRepository.GetByIdWithSongsForUpdateAsync(request.PlaylistId, cancellationToken)
                ?? throw new PlaylistNotFoundException(request.PlaylistId);

            if (playlist.OwnerId != request.RequesterId)
                throw new NotPlaylistOwnerException(request.PlaylistId, request.RequesterId);

            var entry = playlist.SongPlaylists.FirstOrDefault(sp => sp.SongId == request.SongId)
                ?? throw new RedundantOperationException($"Song '{request.SongId}' is not in playlist '{request.PlaylistId}'.");

            await _playlistWriteRepository.RemoveSongPlaylistEntryAsync(entry, cancellationToken);
            await _playlistWriteRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}