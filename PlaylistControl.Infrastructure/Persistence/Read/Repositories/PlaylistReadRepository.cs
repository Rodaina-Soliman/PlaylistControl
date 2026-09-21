using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
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
        public async Task<PagedResult<Playlist>> GetByOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching playlists by owner from repository...");

            var query = _context.Playlists
                .Include(p => p.Owner)
                .Include(p => p.SongPlaylists)
                .Where(p => p.OwnerId == ownerId)
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched playlists by owner from repository.");
            return new PagedResult<Playlist>(items, page, pageSize, totalCount);
        }

        /// <inheritdoc/>
        public async Task<PagedResult<Playlist>> GetUserLibraryAsync(Guid userId, Guid requesterId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching user library playlists from repository...");

            var query = _context.UserPlaylists
                .Where(up => up.UserId == userId
                             && (up.Playlist.IsPublic || up.Playlist.OwnerId == requesterId))
                .OrderBy(up => up.AddedAt)
                .ThenBy(up => up.PlaylistId);

            var totalCount = await query.CountAsync(cancellationToken);
            var playlists = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(up => up.Playlist)
                    .ThenInclude(p => p.Owner)
                .Include(up => up.Playlist)
                    .ThenInclude(p => p.SongPlaylists)
                .Select(up => up.Playlist)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched user library playlists from repository.");
            return new PagedResult<Playlist>(playlists, page, pageSize, totalCount);
        }

        /// <inheritdoc/>
        public async Task<bool> IsInUserLibraryAsync(Guid userId, Guid playlistId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Checking user library membership in repository...");
            var exists = await _context.UserPlaylists.AnyAsync(up => up.UserId == userId && up.PlaylistId == playlistId, cancellationToken);
            _logger.LogInformation("Checked user library membership in repository.");
            return exists;
        }

        public async Task<PagedResult<Playlist>> GetPublicAndOwnedAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching public and owned playlists from repository...");

            var query = _context.Playlists
                .Include(p => p.Owner)
                .Include(p => p.SongPlaylists)
                .Where(p => p.IsPublic || p.OwnerId == ownerId)
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched public and owned playlists from repository.");
            return new PagedResult<Playlist>(items, page, pageSize, totalCount);
        }

        public async Task<PlaylistWithSongsPage> GetPlaylistWithPagedSongsAsync(Guid playlistId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching playlist with paged songs from repository...");

            var playlist = await _context.Playlists
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == playlistId, cancellationToken);

            if (playlist is null)
            {
                _logger.LogInformation("Playlist not found; returning empty page.");
                return new PlaylistWithSongsPage(null, new PagedResult<Song>(Array.Empty<Song>(), page, pageSize, 0));
            }

            var songsQuery = _context.SongPlaylists
                .Where(sp => sp.PlaylistId == playlistId)
                .OrderBy(sp => sp.AddedAt)
                .ThenBy(sp => sp.SongId);

            var totalCount = await songsQuery.CountAsync(cancellationToken);
            var songs = await songsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(sp => sp.Song)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched playlist with paged songs from repository.");
            return new PlaylistWithSongsPage(playlist, new PagedResult<Song>(songs, page, pageSize, totalCount));
        }
    }
}