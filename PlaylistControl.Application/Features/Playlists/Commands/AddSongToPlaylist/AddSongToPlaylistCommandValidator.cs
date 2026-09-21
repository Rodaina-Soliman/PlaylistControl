using FluentValidation;

namespace PlaylistControl.Application.Features.Playlists.Commands.AddSongToPlaylist
{
    /// <summary>
    /// Validator for <see cref="AddSongToPlaylistCommand"/>
    /// </summary>
    public class AddSongToPlaylistCommandValidator : AbstractValidator<AddSongToPlaylistCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddSongToPlaylistCommandValidator"/> class.
        /// </summary>
        public AddSongToPlaylistCommandValidator()
        {
            RuleFor(x => x.RequesterId).NotEmpty();
            RuleFor(x => x.PlaylistId).NotEmpty();
            RuleFor(x => x.SongId).NotEmpty();
        }
    }
}