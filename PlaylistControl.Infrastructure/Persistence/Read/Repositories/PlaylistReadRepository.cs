using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Infrastructure.Persistence.Read.Repositories
{
    /// <summary>
    /// Implementation of IPlaylistReadRepository interface
    /// </summary>
    public class PlaylistReadRepository : IPlaylistReadRepository
    {
        private readonly ILogger<PlaylistReadRepository> _logger;
        private readonly PlaylistReadDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistReadRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="context">Read DbContext</param>
        public PlaylistReadRepository(ILogger<PlaylistReadRepository> logger, PlaylistReadDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<Playlist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching playlist by id from repository...");
            var playlist = await _context.Playlists
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            _logger.LogInformation("Fetched playlist by id from repository.");
            return playlist;
        }

        /// <inheritdoc/>
        public async Task<Playlist?> GetByIdWithSongsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching playlist with songs by id from repository...");
            var playlist = await _context.Playlists
                .Include(p => p.SongPlaylists)
                    .ThenInclude(sp => sp.Song)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            _logger.LogInformation("Fetched playlist with songs by id from repository.");
            return playlist;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Playlist>> GetAllPublicAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching all public playlists from repository...");
            var playlists = await _context.Playlists
                .Include(p => p.Owner)
                .Include(p => p.SongPlaylists)
                .Where(p => p.IsPublic)
                .ToListAsync(cancellationToken);
            _logger.LogInformation("Fetched all public playlists from repository.");
            return playlists;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Playlist>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching playlists by owner from repository...");
            var playlists = await _context.Playlists
                .Include(p => p.Owner)
                .Include(p => p.SongPlaylists)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync(cancellationToken);
            _logger.LogInformation("Fetched playlists by owner from repository.");
            return playlists;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Playlist>> GetUserLibraryAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching user library playlists from repository...");
            var playlists = await _context.UserPlaylists
                .Where(up => up.UserId == userId)
                .Include(up => up.Playlist)
                    .ThenInclude(p => p.Owner)
                .Include(up => up.Playlist)
                    .ThenInclude(p => p.SongPlaylists)
                .Select(up => up.Playlist)
                .ToListAsync(cancellationToken);
            _logger.LogInformation("Fetched user library playlists from repository.");
            return playlists;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Checking playlist existence in repository...");
            var exists = await _context.Playlists
                .AnyAsync(p => p.Id == id, cancellationToken);
            _logger.LogInformation("Checked playlist existence in repository.");
            return exists;
        }
    }
}