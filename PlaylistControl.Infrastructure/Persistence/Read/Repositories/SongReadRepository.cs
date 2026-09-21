using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
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
        public async Task<PagedResult<Song>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching paged songs from repository...");

            var query = _context.Songs
                .OrderBy(s => s.Title)
                .ThenBy(s => s.Id);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched paged songs from repository.");
            return new PagedResult<Song>(items, page, pageSize, totalCount);
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