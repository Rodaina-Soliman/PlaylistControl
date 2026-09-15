using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Infrastructure.Persistence.Write.Repositories
{
    /// <summary>
    /// Implementation of IPlaylistWriteRepository interface
    /// </summary>
    public class PlaylistWriteRepository : IPlaylistWriteRepository
    {
        private readonly ILogger<PlaylistWriteRepository> _logger;
        private readonly PlaylistWriteDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistWriteRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="context">DbContext</param>
        public PlaylistWriteRepository(ILogger<PlaylistWriteRepository> logger, PlaylistWriteDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <inheritdoc/>
        public async Task AddAsync(Playlist playlist, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding playlist to repository...");
            await _context.Playlists.AddAsync(playlist, cancellationToken);
            _logger.LogInformation("Added playlist to repository.");
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(Playlist playlist, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting playlist from repository...");
            _context.Playlists.Remove(playlist);
            await Task.CompletedTask;
            _logger.LogInformation("Deleted playlist from repository.");
        }

        /// <inheritdoc/>
        public async Task<Playlist?> GetByIdWithSongsForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching playlist from repository...");
            var playlist = await _context.Playlists.Include(p => p.SongPlaylists).ThenInclude(sp => sp.Song).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            _logger.LogInformation("Fetched playlist from repository.");
            return playlist;
        }

        /// <inheritdoc/>
        public async Task<Playlist?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching playlist from repository...");
            var playlist = await _context.Playlists.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            _logger.LogInformation("Fetched playlist from repository.");
            return playlist;
        }

        /// <inheritdoc/>
        public async Task<UserPlaylist?> GetUserPlaylistEntryAsync(Guid userId, Guid playlistId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching user-playlist from repository...");
            var playlist = await _context.UserPlaylists.FirstOrDefaultAsync(up => up.UserId == userId && up.PlaylistId == playlistId, cancellationToken);
            _logger.LogInformation("Fetched user-playlist from repository.");
            return playlist;
        }

        /// <inheritdoc/>
        public async Task AddUserPlaylistEntryAsync(UserPlaylist entry, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding user-playlist to repository...");
            await _context.UserPlaylists.AddAsync(entry, cancellationToken);
            _logger.LogInformation("Added user-playlist to repository.");
        }

        /// <inheritdoc/>
        public async Task RemoveUserPlaylistEntryAsync(UserPlaylist entry, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing user-playlist from repository...");
            _context.UserPlaylists.Remove(entry);
            await Task.CompletedTask;
            _logger.LogInformation("Removed user-playlist from repository.");
        }

        /// <inheritdoc/>
        public async Task AddSongPlaylistEntryAsync(SongPlaylist entry, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Adding song-playlist to repository...");
            await _context.SongPlaylists.AddAsync(entry, cancellationToken);
            _logger.LogInformation("Added song-playlist to repository.");
        }

        /// <inheritdoc/>
        public async Task RemoveSongPlaylistEntryAsync(SongPlaylist entry, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Removing song-playlist from repository...");
            _context.SongPlaylists.Remove(entry);
            await Task.CompletedTask;
            _logger.LogInformation("Removed song-playlist from repository.");
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to repository...");
            var result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved changes to repository.");
            return result;
        }
    }
}
