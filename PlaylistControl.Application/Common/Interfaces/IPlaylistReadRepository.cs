using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Application.Common.Interfaces;

/// <summary>
/// Read-only repository interface for Playlist queries
/// </summary>
public interface IPlaylistReadRepository
{
    /// <summary>
    /// Retrieves a Playlist from the Playlists table by its unique identifier, including its Owner
    /// </summary>
    /// <param name="id">Unique identifier of the Playlist</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The Playlist if found, null otherwise</returns>
    Task<Playlist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a Playlist by its unique identifier, including its associated Songs
    /// </summary>
    /// <param name="id">Unique identifier of the Playlist</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The Playlist with its Songs if found, null otherwise</returns>
    Task<Playlist?> GetByIdWithSongsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all public Playlists from the Playlists table
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of public Playlists</returns>
    Task<IReadOnlyList<Playlist>> GetAllPublicAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all Playlists owned by the given User
    /// </summary>
    /// <param name="ownerId">Unique identifier of the owning User</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of Playlists owned by the User</returns>
    Task<IReadOnlyList<Playlist>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all Playlists that appear in the given User's library
    /// </summary>
    /// <param name="userId">Unique identifier of the User whose library is queried</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of Playlists in the User's library</returns>
    Task<IReadOnlyList<Playlist>> GetUserLibraryAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a Playlist with the given identifier exists
    /// </summary>
    /// <param name="id">Unique identifier of the Playlist</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>True if the Playlist exists, false otherwise</returns>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}