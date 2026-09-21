using FluentValidation;

namespace PlaylistControl.Application.Features.Playlists.Commands.UpdatePlaylist
{
    /// <summary>
    /// Validator for <see cref="UpdatePlaylistCommand"/>
    /// </summary>
    public class UpdatePlaylistCommandValidator : AbstractValidator<UpdatePlaylistCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePlaylistCommandValidator"/> class.
        /// </summary>
        public UpdatePlaylistCommandValidator()
        {
            RuleFor(x => x.RequesterId).NotEmpty();
            RuleFor(x => x.PlaylistId).NotEmpty();

            RuleFor(x => x)
                .Must(x => x.Name is not null || x.IsPublic is not null)
                .WithMessage("At least one of 'Name' or 'IsPublic' must be provided.");

            RuleFor(x => x.Name!)
                .NotEmpty()
                .MaximumLength(200)
                .When(x => x.Name is not null);
        }
    }
}