using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Domain.Entities
{
    public class Playlist
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Navigation property
        /// </summary>
        public User Owner { get; set; } = null!;
        /// <summary>
        /// Navigation property
        /// </summary>
        public ICollection<UserPlaylist> UserPlaylists { get; set; } = new List<UserPlaylist>();
        /// <summary>
        /// Navigation property
        /// </summary>
        public ICollection<SongPlaylist> SongPlaylists { get; set; } = new List<SongPlaylist>();
    }
}
