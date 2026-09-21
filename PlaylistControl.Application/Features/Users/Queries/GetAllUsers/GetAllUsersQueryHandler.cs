using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Interfaces;
using PlaylistControl.Application.Common.Models;

namespace PlaylistControl.Application.Features.Users.Queries.GetAllUsers
{
    /// <summary>
    /// Handler for <see cref="GetAllUsersQuery"/>
    /// </summary>
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResult<UserDto>>
    {
        private readonly IUserReadRepository _userReadRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllUsersQueryHandler"/> class.
        /// </summary>
        /// <param name="userReadRepository">Read-only user repository</param>
        public GetAllUsersQueryHandler(IUserReadRepository userReadRepository)
        {
            _userReadRepository = userReadRepository;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var page = await _userReadRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);

            var items = page.Items
                .Select(u => u.ToUserDto())
                .ToList();

            return new PagedResult<UserDto>(
                items, page.Page, page.PageSize, page.TotalCount);
        }
    }
}