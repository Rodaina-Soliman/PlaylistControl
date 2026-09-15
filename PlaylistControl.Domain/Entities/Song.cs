using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Domain.Entities
{
    /// <summary>
    /// Represents instance of Song record
    /// </summary>
    public class Song
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
        /// <summary>
        /// Navigation property
        /// </summary>
        public ICollection<SongPlaylist> SongPlaylists { get; set; } = new List<SongPlaylist>();
    }
}
