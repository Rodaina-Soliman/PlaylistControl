namespace PlaylistControl.Domain.Exceptions
{
    /// <summary>
    /// Exception specifying a redundant operation was attempted
    /// </summary>
    public class RedundantOperationException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedundantOperationException"/> class.
        /// </summary>
        /// <param name="message">Message that specifies error details</param>
        public RedundantOperationException(string message) : base(message) { }
    }
}