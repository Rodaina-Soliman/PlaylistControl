using MediatR;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.Application.Features.Playlists.Commands.UpdatePlaylist
{
    /// <summary>
    /// Handler for <see cref="UpdatePlaylistCommand"/>
    /// </summary>
    public class UpdatePlaylistCommandHandler : IRequestHandler<UpdatePlaylistCommand, bool>
    {
        private readonly IPlaylistWriteRepository _playlistWriteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePlaylistCommandHandler"/> class.
        /// </summary>
        /// <param name="playlistWriteRepository">Write-side playlist repository</param>
        public UpdatePlaylistCommandHandler(IPlaylistWriteRepository playlistWriteRepository)
        {
            _playlistWriteRepository = playlistWriteRepository;
        }

        /// <inheritdoc/>
        public async Task<bool> Handle(UpdatePlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _playlistWriteRepository.GetByIdForUpdateAsync(request.PlaylistId, cancellationToken)
                ?? throw new PlaylistNotFoundException(request.PlaylistId);

            if (playlist.OwnerId != request.RequesterId)
                throw new NotPlaylistOwnerException(request.PlaylistId, request.RequesterId);

            if (request.Name is not null)
                playlist.Name = request.Name;

            if (request.IsPublic is not null)
                playlist.IsPublic = request.IsPublic.Value;

            await _playlistWriteRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}