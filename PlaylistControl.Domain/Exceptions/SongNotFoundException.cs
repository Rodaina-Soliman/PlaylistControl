namespace PlaylistControl.Domain.Exceptions
{
    /// <summary>
    /// Exception specifying song was not found
    /// </summary>
    public class SongNotFoundException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SongNotFoundException"/> class.
        /// </summary>
        public SongNotFoundException(Guid songId) : base($"Song with ID '{songId}' was not found.") { }
    }
}
