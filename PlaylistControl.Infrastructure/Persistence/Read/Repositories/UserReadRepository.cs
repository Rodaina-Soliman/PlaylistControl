using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Infrastructure.Persistence.Read.Repositories
{
    /// <summary>
    /// Implementation of IUserReadRepository interface
    /// </summary>
    public class UserReadRepository : IUserReadRepository
    {
        private readonly ILogger<UserReadRepository> _logger;
        private readonly PlaylistReadDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserReadRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="context">Read DbContext</param>
        public UserReadRepository(ILogger<UserReadRepository> logger, PlaylistReadDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<User>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching paged users from repository...");

            var query = _context.Users
                .OrderBy(u => u.Username)
                .ThenBy(u => u.Id);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched paged users from repository.");
            return new PagedResult<User>(items, page, pageSize, totalCount);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Checking user existence in repository...");
            var exists = await _context.Users
                .AnyAsync(u => u.Id == id, cancellationToken);
            _logger.LogInformation("Checked user existence in repository.");
            return exists;
        }
    }
}