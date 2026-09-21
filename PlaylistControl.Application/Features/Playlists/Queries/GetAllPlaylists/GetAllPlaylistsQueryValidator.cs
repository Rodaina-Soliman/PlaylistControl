using FluentValidation;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetAllPlaylists
{
    /// <summary>
    /// Validator for <see cref="GetAllPlaylistsQuery"/>
    /// </summary>
    public class GetAllPlaylistsQueryValidator : AbstractValidator<GetAllPlaylistsQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllPlaylistsQueryValidator"/> class.
        /// </summary>
        public GetAllPlaylistsQueryValidator()
        {
            RuleFor(x => x.RequesterId).NotEmpty();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
        }
    }
}