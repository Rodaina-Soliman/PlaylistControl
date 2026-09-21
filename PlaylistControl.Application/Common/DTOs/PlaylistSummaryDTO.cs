using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Application.Common.DTOs
{
    /// <summary>
    /// Summary representation of a Playlist
    /// </summary>
    /// <param name="Id">Unique identifier of the Playlist</param>
    /// <param name="Name">Name of the Playlist</param>
    /// <param name="IsPublic">Whether the Playlist is public</param>
    /// <param name="OwnerId">Unique identifier of the owning User</param>
    /// <param name="OwnerUsername">Username of the owning User</param>
    /// <param name="SongCount">Number of Songs in the Playlist</param>
    public record PlaylistSummaryDto(
        Guid Id,
        string Name,
        bool IsPublic,
        Guid OwnerId,
        string OwnerUsername,
        int SongCount);

    /// <summary>
    /// Extension methods for mapping <see cref="Playlist"/> entities to <see cref="PlaylistSummaryDto"/>
    /// </summary>
    public static class PlaylistSummaryDtoExtensions
    {
        /// <summary>
        /// Projects a <see cref="Playlist"/> into a <see cref="PlaylistSummaryDto"/>
        /// </summary>
        /// <param name="playlist">Playlist entity to project</param>
        /// <returns>A new <see cref="PlaylistSummaryDto"/></returns>
        public static PlaylistSummaryDto ToSummaryDto(this Playlist playlist)
            => new(
                playlist.Id,
                playlist.Name,
                playlist.IsPublic,
                playlist.OwnerId,
                playlist.Owner?.Username ?? string.Empty,
                playlist.SongPlaylists.Count);
    }
}
