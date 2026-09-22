using MediatR;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.Application.Features.Playlists.Commands.RemovePlaylistFromUser
{
    /// <summary>
    /// Handles <see cref="RemovePlaylistFromUserCommand"/>
    /// </summary>
    public class RemovePlaylistFromUserCommandHandler : IRequestHandler<RemovePlaylistFromUserCommand, bool>
    {
        private readonly IPlaylistWriteRepository _writeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovePlaylistFromUserCommandHandler"/> class.
        /// </summary>
        /// <param name="writeRepository">Write-side playlist repository</param>
        public RemovePlaylistFromUserCommandHandler(IPlaylistWriteRepository writeRepository)
        {
            _writeRepository = writeRepository;
        }

        /// <inheritdoc />
        public async Task<bool> Handle(RemovePlaylistFromUserCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _writeRepository.GetByIdForUpdateAsync(request.PlaylistId, cancellationToken)
                ?? throw new PlaylistNotFoundException(request.PlaylistId);

            if (request.RequesterId == playlist.OwnerId)
            {
                throw new RedundantOperationException(
                    "The playlist owner cannot remove their own playlist from their library; delete it instead.");
            }

            var existing = await _writeRepository.GetUserPlaylistEntryAsync(
                request.RequesterId, request.PlaylistId, cancellationToken);

            if (existing is null)
            {
                throw new RedundantOperationException(
                    "The playlist is not in the requester's library.");
            }

            await _writeRepository.RemoveUserPlaylistEntryAsync(existing, cancellationToken);
            await _writeRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}