using FluentValidation;

namespace PlaylistControl.Application.Features.Songs.Queries.GetAllSongs
{
    /// <summary>
    /// Validator for <see cref="GetAllSongsQuery"/>
    /// </summary>
    public class GetAllSongsQueryValidator : AbstractValidator<GetAllSongsQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllSongsQueryValidator"/> class.
        /// </summary>
        public GetAllSongsQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
        }
    }
}