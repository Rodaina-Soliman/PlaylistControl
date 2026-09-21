using MediatR;
using PlaylistControl.Application.Common.DTOs;
using PlaylistControl.Application.Common.Models;

namespace PlaylistControl.Application.Features.Users.Queries.GetAllUsers
{
    /// <summary>
    /// Query for a page of the static User catalog
    /// </summary>
    /// <param name="Page">1-based page number</param>
    /// <param name="PageSize">Number of items per page</param>
    public record GetAllUsersQuery(
        int Page = 1,
        int PageSize = 1) : IRequest<PagedResult<UserDto>>;
}