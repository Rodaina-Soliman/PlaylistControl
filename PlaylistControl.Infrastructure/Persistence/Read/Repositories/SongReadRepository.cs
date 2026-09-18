using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Infrastructure.Persistence.Read.Repositories
{
    /// <summary>
    /// Implementation of ISongReadRepository interface
    /// </summary>
    public class SongReadRepository : ISongReadRepository
    {
        private readonly ILogger<SongReadRepository> _logger;
        private readonly PlaylistReadDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SongReadRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="context">Read DbContext</param>
        public SongReadRepository(ILogger<SongReadRepository> logger, PlaylistReadDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<Song?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching song by id from repository...");
            var song = await _context.Songs
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
            _logger.LogInformation("Fetched song by id from repository.");
            return song;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Song>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching all songs from repository...");
            var songs = await _context.Songs
                .ToListAsync(cancellationToken);
            _logger.LogInformation("Fetched all songs from repository.");
            return songs;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Checking song existence in repository...");
            var exists = await _context.Songs
                .AnyAsync(s => s.Id == id, cancellationToken);
            _logger.LogInformation("Checked song existence in repository.");
            return exists;
        }
    }
}