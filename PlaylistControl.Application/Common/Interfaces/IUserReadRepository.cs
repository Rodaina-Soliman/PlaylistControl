using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Application.Common.Interfaces;

/// <summary>
/// Read-only repository interface for User lookups.
/// </summary>
public interface IUserReadRepository
{
    /// <summary>
    /// Retrieves a User from the Users table by its unique identifier.
    /// </summary>
    /// <param name="id">Unique identifier of the User.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The User if found, null otherwise.</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all Users from the Users table.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>List of all Users.</returns>
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a User with the given identifier exists.
    /// </summary>
    /// <param name="id">Unique identifier of the User.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the User exists, false otherwise.</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}