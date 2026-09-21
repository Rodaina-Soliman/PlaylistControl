using MediatR;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Domain.Entities;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.Application.Features.Playlists.Commands.AddSongToPlaylist
{
    /// <summary>
    /// Handler for <see cref="AddSongToPlaylistCommand"/>
    /// </summary>
    public class AddSongToPlaylistCommandHandler : IRequestHandler<AddSongToPlaylistCommand, bool>
    {
        private readonly ISongReadRepository _songReadRepository;
        private readonly IPlaylistWriteRepository _playlistWriteRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSongToPlaylistCommandHandler"/> class.
        /// </summary>
        /// <param name="songReadRepository">Read-only song repository</param>
        /// <param name="playlistWriteRepository">Write-side playlist repository</param>
        public AddSongToPlaylistCommandHandler(
            ISongReadRepository songReadRepository,
            IPlaylistWriteRepository playlistWriteRepository)
        {
            _songReadRepository = songReadRepository;
            _playlistWriteRepository = playlistWriteRepository;
        }

        /// <inheritdoc/>
        public async Task<bool> Handle(AddSongToPlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _playlistWriteRepository.GetByIdWithSongsForUpdateAsync(request.PlaylistId, cancellationToken)
                ?? throw new PlaylistNotFoundException(request.PlaylistId);

            if (playlist.OwnerId != request.RequesterId)
                throw new NotPlaylistOwnerException(request.PlaylistId, request.RequesterId);

            if (!await _songReadRepository.ExistsAsync(request.SongId, cancellationToken))
                throw new SongNotFoundException(request.SongId);

            if (playlist.SongPlaylists.Any(sp => sp.SongId == request.SongId))
                throw new RedundantOperationException($"Song '{request.SongId}' is already in playlist '{request.PlaylistId}'.");

            await _playlistWriteRepository.AddSongPlaylistEntryAsync(new SongPlaylist
            {
                SongId = request.SongId,
                PlaylistId = request.PlaylistId,
                AddedAt = DateTime.UtcNow
            }, cancellationToken);

            await _playlistWriteRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}