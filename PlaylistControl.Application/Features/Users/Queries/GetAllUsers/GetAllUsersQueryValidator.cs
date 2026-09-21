using FluentValidation;

namespace PlaylistControl.Application.Features.Users.Queries.GetAllUsers
{
    /// <summary>
    /// Validator for <see cref="GetAllUsersQuery"/>
    /// </summary>
    public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllUsersQueryValidator"/> class.
        /// </summary>
        public GetAllUsersQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
        }
    }
}