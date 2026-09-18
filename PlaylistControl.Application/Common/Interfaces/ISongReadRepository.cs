using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Application.Common.Interfaces;

/// <summary>
/// Read-only repository interface for Song lookups
/// </summary>
public interface ISongReadRepository
{
    /// <summary>
    /// Retrieves a Song from the Songs table by its unique identifier
    /// </summary>
    /// <param name="id">Unique identifier of the Song</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The Song if found, null otherwise</returns>
    Task<Song?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all Songs from the Songs table
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of all Songs</returns>
    Task<IReadOnlyList<Song>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a Song with the given identifier exists
    /// </summary>
    /// <param name="id">Unique identifier of the Song</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>True if the Song exists, false otherwise</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}