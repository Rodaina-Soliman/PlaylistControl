using FluentValidation;

namespace PlaylistControl.Application.Features.Playlists.Commands.RemoveSongFromPlaylist
{
    /// <summary>
    /// Validator for <see cref="RemoveSongFromPlaylistCommand"/>
    /// </summary>
    public class RemoveSongFromPlaylistCommandValidator : AbstractValidator<RemoveSongFromPlaylistCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveSongFromPlaylistCommandValidator"/> class.
        /// </summary>
        public RemoveSongFromPlaylistCommandValidator()
        {
            RuleFor(x => x.RequesterId).NotEmpty();
            RuleFor(x => x.PlaylistId).NotEmpty();
            RuleFor(x => x.SongId).NotEmpty();
        }
    }
}