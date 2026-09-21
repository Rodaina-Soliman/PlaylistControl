namespace PlaylistControl.Domain.Exceptions
{
    /// <summary>
    /// Exception specifying playlist was not found
    /// </summary>
    public class PlaylistNotFoundException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistNotFoundException"/> class.
        /// </summary>
        public PlaylistNotFoundException(Guid playlistId) : base($"Playlist with ID '{playlistId}' was not found.") { }
    }
}
