using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlaylistControl.Api.Extensions;
using PlaylistControl.Application.Features.Playlists.Commands.AddPlaylistToUser;
using PlaylistControl.Application.Features.Playlists.Commands.RemovePlaylistFromUser;
using PlaylistControl.Application.Features.Playlists.Queries.GetMyPlaylists;
using PlaylistControl.Application.Features.Playlists.Queries.GetUserPlaylists;
using PlaylistControl.Application.Features.Users.Queries.GetAllUsers;

namespace PlaylistControl.Api.Controllers
{
    /// <summary>
    /// User endpoints.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="mediator">MediatR mediator</param>
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Lists the static user catalog.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 1,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllUsersQuery(page, pageSize), cancellationToken);
            return Ok(result);
        }

        /// <summary>Lists playlists owned by the requester.</summary>
        [HttpGet("me/playlists")]
        public async Task<IActionResult> GetMine(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 1,
            CancellationToken cancellationToken = default)
        {
            var requesterId = HttpContext.GetRequesterId();
            var result = await _mediator.Send(
                new GetMyPlaylistsQuery(requesterId, page, pageSize), cancellationToken);
            return Ok(result);
        }

        /// <summary>Lists playlists in another user's library.</summary>
        [HttpGet("{userId:guid}/playlists")]
        public async Task<IActionResult> GetForUser(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 1,
            CancellationToken cancellationToken = default)
        {
            var requesterId = HttpContext.GetRequesterId();
            var result = await _mediator.Send(
                new GetUserPlaylistsQuery(requesterId, userId, page, pageSize), cancellationToken);
            return Ok(result);
        }

        /// <summary>Adds a playlist to the requester's own library.</summary>
        [HttpPost("me/playlists/{id:guid}")]
        public async Task<IActionResult> AddToMyLibrary(Guid id, CancellationToken cancellationToken)
        {
            var requesterId = HttpContext.GetRequesterId();
            await _mediator.Send(
                new AddPlaylistToUserCommand(requesterId, requesterId, id), cancellationToken);
            return NoContent();
        }

        /// <summary>Removes a playlist from the requester's own library.</summary>
        [HttpDelete("me/playlists/{id:guid}")]
        public async Task<IActionResult> RemoveFromMyLibrary(Guid id, CancellationToken cancellationToken)
        {
            var requesterId = HttpContext.GetRequesterId();
            await _mediator.Send(new RemovePlaylistFromUserCommand(requesterId, id), cancellationToken);
            return NoContent();
        }
    }
}