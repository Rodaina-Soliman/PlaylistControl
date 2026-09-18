using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Application.Common.Interfaces;

/// <summary>
/// Write-side repository interface for Playlist entities
/// </summary>
public interface IPlaylistWriteRepository
{
    /// <summary>
    /// Adds a new Playlist to the Playlists table
    /// </summary>
    /// <param name="playlist">Playlist information to add</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    Task AddAsync(Playlist playlist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a Playlist for deletion from the Playlists table
    /// </summary>
    /// <param name="playlist">Playlist entity to delete</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    Task DeleteAsync(Playlist playlist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a tracked Playlist by its unique identifier, without its Songs
    /// </summary>
    /// <param name="id">Unique identifier of the Playlist</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The tracked Playlist if found, null otherwise</returns>
    Task<Playlist?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a tracked Playlist by its unique identifier, including its associated Songs
    /// </summary>
    /// <param name="id">Unique identifier of the Playlist</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The tracked Playlist with its Songs if found, null otherwise</returns>
    Task<Playlist?> GetByIdWithSongsForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a tracked UserPlaylist join row for the given User and Playlist
    /// </summary>
    /// <param name="userId">Unique identifier of the User</param>
    /// <param name="playlistId">Unique identifier of the Playlist</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The UserPlaylist entry if found, null otherwise</returns>
    Task<UserPlaylist?> GetUserPlaylistEntryAsync(Guid userId, Guid playlistId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new UserPlaylist join row, linking a User to a Playlist in their library
    /// </summary>
    /// <param name="entry">UserPlaylist entry to add</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    Task AddUserPlaylistEntryAsync(UserPlaylist entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a UserPlaylist join row for deletion
    /// </summary>
    /// <param name="entry">UserPlaylist entry to delete</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    Task RemoveUserPlaylistEntryAsync(UserPlaylist entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new SongPlaylist join row, linking a Song to a Playlist
    /// </summary>
    /// <param name="entry">SongPlaylist entry to add</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    Task AddSongPlaylistEntryAsync(SongPlaylist entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a SongPlaylist join row for deletion
    /// </summary>
    /// <param name="entry">SongPlaylist entry to delete</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    Task RemoveSongPlaylistEntryAsync(SongPlaylist entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits all pending changes tracked by the write context to the database
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}