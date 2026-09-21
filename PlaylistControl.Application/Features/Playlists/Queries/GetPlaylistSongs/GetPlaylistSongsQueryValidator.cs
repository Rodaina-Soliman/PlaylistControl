using FluentValidation;

namespace PlaylistControl.Application.Features.Playlists.Queries.GetPlaylistSongs
{
    /// <summary>
    /// Validator for <see cref="GetPlaylistSongsQuery"/>
    /// </summary>
    public class GetPlaylistSongsQueryValidator : AbstractValidator<GetPlaylistSongsQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetPlaylistSongsQueryValidator"/> class.
        /// </summary>
        public GetPlaylistSongsQueryValidator()
        {
            RuleFor(x => x.RequesterId).NotEmpty();
            RuleFor(x => x.PlaylistId).NotEmpty();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
        }
    }
}