using PlaylistControl.Domain.Entities;

namespace PlaylistControl.Application.Common.DTOs
{
    /// <summary>
    /// Representation of a Song
    /// </summary>
    /// <param name="Id">Unique identifier of the Song</param>
    /// <param name="Title">Title of the Song</param>
    /// <param name="Artist">Artist of the Song</param>
    /// <param name="DurationSeconds">Duration of the Song in seconds</param>
    public record SongDto(
        Guid Id,
        string Title,
        string Artist,
        int DurationSeconds);

    /// <summary>
    /// Extension methods for mapping <see cref="Song"/> entities to <see cref="SongDto"/>
    /// </summary>
    public static class SongDtoExtensions
    {
        /// <summary>
        /// Projects a <see cref="Song"/> into a <see cref="SongDto"/>
        /// </summary>
        /// <param name="song">Song entity to project</param>
        /// <returns>A new <see cref="SongDto"/></returns>
        public static SongDto ToSongDto(this Song song)
            => new(
                song.Id,
                song.Title,
                song.Artist,
                song.DurationSeconds);
    }
}