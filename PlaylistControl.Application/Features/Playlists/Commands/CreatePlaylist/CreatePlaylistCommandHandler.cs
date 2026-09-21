using MediatR;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.Application.Features.Playlists.Commands.CreatePlaylist
{
    /// <summary>
    /// Handler for <see cref="CreatePlaylistCommand"/>
    /// </summary>
    public class CreatePlaylistCommandHandler : IRequestHandler<CreatePlaylistCommand, Guid>
    {
        private readonly IUserReadRepository _userReadRepository;
        private readonly IPlaylistWriteRepository _playlistWriteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePlaylistCommandHandler"/> class.
        /// </summary>
        /// <param name="userReadRepository">Read-only user repository</param>
        /// <param name="playlistWriteRepository">Write-side playlist repository</param>
        public CreatePlaylistCommandHandler(IUserReadRepository userReadRepository, IPlaylistWriteRepository playlistWriteRepository)
        {
            _userReadRepository = userReadRepository;
            _playlistWriteRepository = playlistWriteRepository;
        }

        /// <inheritdoc/>
        public async Task<Guid> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
        {
            if (!await _userReadRepository.ExistsAsync(request.RequesterId, cancellationToken))
                throw new UserNotFoundException(request.RequesterId);

            var playlist = new Playlist
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                IsPublic = request.IsPublic,
                OwnerId = request.RequesterId,
                CreatedAt = DateTime.UtcNow
            };

            await _playlistWriteRepository.AddAsync(playlist, cancellationToken);

            await _playlistWriteRepository.AddUserPlaylistEntryAsync(new UserPlaylist
            {
                UserId = request.RequesterId,
                PlaylistId = playlist.Id,
                AddedAt = DateTime.UtcNow
            }, cancellationToken);

            await _playlistWriteRepository.SaveChangesAsync(cancellationToken);

            return playlist.Id;
        }
    }
}