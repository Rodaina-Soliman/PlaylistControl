using PlaylistControl.Application.Common.Models;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Application.Common.Interfaces;

/// <summary>
/// Read-only repository interface for User lookups
/// </summary>
public interface IUserReadRepository
{

    /// <summary>
    /// Retrieves all Users from the Users table
    /// </summary>
    /// <param name="page">Requested page number</param>
    /// <param name="pageSize">Requested page size</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of all Users</returns>
    Task<PagedResult<User>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a User with the given identifier exists
    /// </summary>
    /// <param name="id">Unique identifier of the User</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>True if the User exists, false otherwise</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}