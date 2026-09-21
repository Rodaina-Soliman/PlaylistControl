using MediatR;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.Application.Features.Playlists.Commands.DeletePlaylist
{
    /// <summary>
    /// Handler for <see cref="DeletePlaylistCommand"/>
    /// </summary>
    public class DeletePlaylistCommandHandler : IRequestHandler<DeletePlaylistCommand, bool>
    {
        private readonly IPlaylistWriteRepository _playlistWriteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePlaylistCommandHandler"/> class.
        /// </summary>
        /// <param name="playlistWriteRepository">Write-side playlist repository</param>
        public DeletePlaylistCommandHandler(IPlaylistWriteRepository playlistWriteRepository)
        {
            _playlistWriteRepository = playlistWriteRepository;
        }

        /// <inheritdoc/>
        public async Task<bool> Handle(DeletePlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _playlistWriteRepository.GetByIdForUpdateAsync(request.PlaylistId, cancellationToken)
                ?? throw new PlaylistNotFoundException(request.PlaylistId);

            if (playlist.OwnerId != request.RequesterId)
                throw new NotPlaylistOwnerException(request.PlaylistId, request.RequesterId);

            await _playlistWriteRepository.DeleteAsync(playlist, cancellationToken);
            await _playlistWriteRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}