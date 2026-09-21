using FluentValidation;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetMyPlaylists
{
    /// <summary>
    /// Validator for <see cref="GetMyPlaylistsQuery"/>
    /// </summary>
    public class GetMyPlaylistsQueryValidator : AbstractValidator<GetMyPlaylistsQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetMyPlaylistsQueryValidator"/> class.
        /// </summary>
        public GetMyPlaylistsQueryValidator()
        {
            RuleFor(x => x.RequesterId).NotEmpty();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
        }
    }
}