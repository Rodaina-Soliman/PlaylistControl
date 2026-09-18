namespace PlaylistControl.Domain.Exceptions
{
    /// <summary>
    /// Exception specifying user was not found
    /// </summary>
    public class UserNotFoundException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        public UserNotFoundException(Guid userId) : base($"User with ID '{userId}' was not found.") { }
    }
}
