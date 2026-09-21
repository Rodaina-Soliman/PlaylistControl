using FluentValidation;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetUserPlaylists
{
    /// <summary>
    /// Validator for <see cref="GetUserPlaylistsQuery"/>
    /// </summary>
    public class GetUserPlaylistsQueryValidator : AbstractValidator<GetUserPlaylistsQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserPlaylistsQueryValidator"/> class.
        /// </summary>
        public GetUserPlaylistsQueryValidator()
        {
            RuleFor(x => x.RequesterId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
        }
    }
}