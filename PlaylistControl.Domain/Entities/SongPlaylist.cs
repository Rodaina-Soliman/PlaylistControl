using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Domain.Entities
{
    public class SongPlaylist
    {
        public Guid SongId { get; set; }
        public Guid PlaylistId { get; set; }
        public DateTime AddedAt { get; set; }
        /// <summary>
        /// Navigation property
        /// </summary>
        public Song Song { get; set; } = null!;
        /// <summary>
        /// Navigation property
        /// </summary>
        public Playlist Playlist { get; set; } = null!;
    }
}
