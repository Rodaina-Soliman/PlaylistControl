using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Domain.Entities
{
    public class UserPlaylist
    {
        public Guid UserId { get; set; }
        public Guid PlaylistId { get; set; }
        public DateTime AddedAt { get; set; }
        /// <summary>
        /// Navigation property
        /// </summary>
        public User User { get; set; } = null!;
        /// <summary>
        /// Navigation property
        /// </summary>
        public Playlist Playlist { get; set; } = null!;
    }
}
