using PlaylistControl.Application.Common.Models;
using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Application.Common.Interfaces;

/// <summary>
/// Read-only repository interface for Song lookups
/// </summary>
public interface ISongReadRepository
{
    /// <summary>
    /// Retrieves all Songs from the Songs table
    /// </summary>
    /// <param name="page">Requested page number</param>
    /// <param name="pageSize">Requested page size</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of all Songs</returns>
    Task<PagedResult<Song>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a Song with the given identifier exists
    /// </summary>
    /// <param name="id">Unique identifier of the Song</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>True if the Song exists, false otherwise</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}