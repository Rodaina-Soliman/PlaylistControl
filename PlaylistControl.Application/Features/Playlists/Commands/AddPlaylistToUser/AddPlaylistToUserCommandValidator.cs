using FluentValidation;

namespace PlaylistControl.Application.Features.Playlists.Commands.AddPlaylistToUser
{
    /// <summary>
    /// Validator for <see cref="AddPlaylistToUserCommand"/>
    /// </summary>
    public class AddPlaylistToUserCommandValidator : AbstractValidator<AddPlaylistToUserCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddPlaylistToUserCommandValidator"/> class.
        /// </summary>
        public AddPlaylistToUserCommandValidator()
        {
            RuleFor(x => x.RequesterId).NotEmpty();
            RuleFor(x => x.TargetUserId).NotEmpty();
            RuleFor(x => x.PlaylistId).NotEmpty();
        }
    }
}