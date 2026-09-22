using MediatR;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.Application.Features.Playlists.Commands.AddPlaylistToUser
{
    /// <summary>
    /// Handles <see cref="AddPlaylistToUserCommand"/>
    /// </summary>
    public class AddPlaylistToUserCommandHandler : IRequestHandler<AddPlaylistToUserCommand, bool>
    {
        private readonly IPlaylistWriteRepository _writeRepository;
        private readonly IUserReadRepository _userReadRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddPlaylistToUserCommandHandler"/> class.
        /// </summary>
        /// <param name="writeRepository">Write-side playlist repository</param>
        /// <param name="userReadRepository">Read-only user repository</param>
        public AddPlaylistToUserCommandHandler(
            IPlaylistWriteRepository writeRepository,
            IUserReadRepository userReadRepository)
        {
            _writeRepository = writeRepository;
            _userReadRepository = userReadRepository;
        }

        /// <inheritdoc />
        public async Task<bool> Handle(AddPlaylistToUserCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _writeRepository.GetByIdForUpdateAsync(request.PlaylistId, cancellationToken)
                ?? throw new PlaylistNotFoundException(request.PlaylistId);

            if (!await _userReadRepository.ExistsAsync(request.TargetUserId, cancellationToken))
            {
                throw new UserNotFoundException(request.TargetUserId);
            }

            if (!playlist.IsPublic && request.RequesterId != playlist.OwnerId)
            {
                throw new PrivatePlaylistAccessException(request.PlaylistId);
            }

            if (request.TargetUserId == playlist.OwnerId)
            {
                throw new RedundantOperationException(
                    "The playlist owner is already a member of their own playlist.");
            }

            var existing = await _writeRepository.GetUserPlaylistEntryAsync(
                request.TargetUserId, request.PlaylistId, cancellationToken);

            if (existing is not null)
            {
                throw new RedundantOperationException(
                    "The playlist is already in the target user's library.");
            }

            var userPlaylist = new UserPlaylist {
                UserId = request.TargetUserId,
                PlaylistId = playlist.Id
            };

            await _writeRepository.AddUserPlaylistEntryAsync(userPlaylist, cancellationToken);
            await _writeRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}