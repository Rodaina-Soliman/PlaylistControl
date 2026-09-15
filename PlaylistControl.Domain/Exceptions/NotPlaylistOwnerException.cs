using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Domain.Exceptions
{
    /// <summary>
    /// Exception specifying user is not playlist owner
    /// </summary>
    public class NotPlaylistOwnerException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotPlaylistOwnerException"/> class.
        /// </summary>
        public NotPlaylistOwnerException(Guid playlistId, Guid userId) : base($"User '{userId}' is not the owner of playlist '{playlistId}'.") { }
    }
}
