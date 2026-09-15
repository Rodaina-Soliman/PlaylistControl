using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Domain.Exceptions
{
    /// <summary>
    /// Exception specifying playlist is private and user is not owner
    /// </summary>
    public class PrivatePlaylistAccessException : DomainException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivatePlaylistAccessException"/> class.
        /// </summary>
        public PrivatePlaylistAccessException(Guid playlistId) : base($"Playlist '{playlistId}' is private and cannot be accessed.") { }
    }
}
