using FluentValidation;

namespace PlaylistControl.Application.Features.Playlists.Commands.CreatePlaylist
{
    /// <summary>
    /// Validator for <see cref="CreatePlaylistCommand"/>
    /// </summary>
    public class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePlaylistCommandValidator"/> class.
        /// </summary>
        public CreatePlaylistCommandValidator()
        {
            RuleFor(x => x.RequesterId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        }
    }
}