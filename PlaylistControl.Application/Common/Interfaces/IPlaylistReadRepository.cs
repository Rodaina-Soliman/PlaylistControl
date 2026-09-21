using PlaylistControl.Application.Common.Models;
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
    /// Retrieves all Playlists owned by the given User
    /// </summary>
    /// <param name="ownerId">Unique identifier of the owning User</param>
    /// <param name="page">Requested page number</param>
    /// <param name="pageSize">Requested page size</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of Playlists owned by the User</returns>
    Task<PagedResult<Playlist>> GetByOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all Playlists that appear in the given User's library
    /// </summary>
    /// <param name="userId">Unique identifier of the User whose library is queried</param>
    /// <param name="requesterId">Unique identifier of the User requesting the library</param>
    /// <param name="page">Requested page number</param>
    /// <param name="pageSize">Requested page size</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of Playlists in the User's library</returns>
    Task<PagedResult<Playlist>> GetUserLibraryAsync(Guid userId, Guid requesterId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the given Playlist appears in the given User's library
    /// </summary>
    /// <param name="userId">Unique identifier of the User</param>
    /// <param name="playlistId">Unique identifier of the Playlist</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>True if the Playlist is in the User's library, false otherwise</returns>
    Task<bool> IsInUserLibraryAsync(Guid userId, Guid playlistId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a page of Playlists that are public or owned by the given User
    /// </summary>
    /// <param name="ownerId">Unique identifier of the User whose library is queried</param>
    /// <param name="page">Requested page number</param>
    /// <param name="pageSize">Requested page size</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of Playlists in the User's library</returns>
    Task<PagedResult<Playlist>> GetPublicAndOwnedAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a Playlist's metadata together with a page of its Songs, ordered by date added
    /// </summary>
    /// <param name="playlistId">Unique identifier of the Playlist</param>
    /// <param name="page">Requested page number</param>
    /// <param name="pageSize">Requested page size</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>List of Songs in the User's Playlist</returns>
    Task<PlaylistWithSongsPage> GetPlaylistWithPagedSongsAsync(Guid playlistId, int page, int pageSize, CancellationToken cancellationToken = default);
}