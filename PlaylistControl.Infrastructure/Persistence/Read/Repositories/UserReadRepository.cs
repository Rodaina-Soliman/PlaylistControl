using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlaylistControl.Application.Common.Interfaces;
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
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching user by id from repository...");
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            _logger.LogInformation("Fetched user by id from repository.");
            return user;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching all users from repository...");
            var users = await _context.Users
                .ToListAsync(cancellationToken);
            _logger.LogInformation("Fetched all users from repository.");
            return users;
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