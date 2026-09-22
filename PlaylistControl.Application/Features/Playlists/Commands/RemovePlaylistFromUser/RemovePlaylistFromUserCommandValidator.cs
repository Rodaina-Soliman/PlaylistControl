using FluentValidation;

namespace PlaylistControl.Application.Features.Playlists.Commands.RemovePlaylistFromUser
{
    /// <summary>
    /// Validator for <see cref="RemovePlaylistFromUserCommand"/>
    /// </summary>
    public class RemovePlaylistFromUserCommandValidator : AbstractValidator<RemovePlaylistFromUserCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovePlaylistFromUserCommandValidator"/> class.
        /// </summary>
        public RemovePlaylistFromUserCommandValidator()
        {
            RuleFor(x => x.RequesterId).NotEmpty();
            RuleFor(x => x.PlaylistId).NotEmpty();
        }
    }
}