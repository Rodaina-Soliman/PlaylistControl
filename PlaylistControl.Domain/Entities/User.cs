using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Domain.Entities
{
    /// <summary>
    /// Represents instance of User record
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Navigation property
        /// </summary>
        public ICollection<UserPlaylist> UserPlaylists { get; set; } = new List<UserPlaylist>();
    }
}
