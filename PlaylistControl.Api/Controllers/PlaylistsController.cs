using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlaylistControl.Api.Extensions;
using PlaylistControl.Application.Features.Playlists.Commands.AddPlaylistToUser;
using PlaylistControl.Application.Features.Playlists.Commands.AddSongToPlaylist;
using PlaylistControl.Application.Features.Playlists.Commands.CreatePlaylist;
using PlaylistControl.Application.Features.Playlists.Commands.DeletePlaylist;
using PlaylistControl.Application.Features.Playlists.Commands.RemoveSongFromPlaylist;
using PlaylistControl.Application.Features.Playlists.Commands.UpdatePlaylist;
using PlaylistControl.Application.Features.Playlists.Queries.GetAllPlaylists;
using PlaylistControl.Application.Features.Playlists.Queries.GetPlaylistSongs;

namespace PlaylistControl.Api.Controllers
{
    /// <summary>
    /// Playlist endpoints.
    /// </summary>
    [ApiController]
    [Route("api/playlists")]
    public class PlaylistsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistsController"/> class.
        /// </summary>
        /// <param name="mediator">MediatR mediator</param>
        public PlaylistsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Request body for creating a playlist.</summary>
        public record CreatePlaylistRequest(string Name, bool IsPublic);

        /// <summary>Request body for updating a playlist.</summary>
        public record UpdatePlaylistRequest(string? Name, bool? IsPublic);

        /// <summary>Request body for adding a song.</summary>
        public record AddSongRequest(Guid SongId);

        /// <summary>Request body for adding a member.</summary>
        public record AddMemberRequest(Guid UserId);

        /// <summary>Creates a playlist owned by the requester.</summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreatePlaylistRequest body,
            CancellationToken cancellationToken)
        {
            var requesterId = HttpContext.GetRequesterId();
            var id = await _mediator.Send(
                new CreatePlaylistCommand(requesterId, body.Name, body.IsPublic), cancellationToken);
            return CreatedAtAction(nameof(GetSongs), new { id }, id);
        }

        /// <summary>Updates a playlist's name and/or privacy.</summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdatePlaylistRequest body,
            CancellationToken cancellationToken)
        {
            var requesterId = HttpContext.GetRequesterId();
            await _mediator.Send(
                new UpdatePlaylistCommand(requesterId, id, body.Name, body.IsPublic), cancellationToken);
            return NoContent();
        }

        /// <summary>Deletes a playlist owned by the requester.</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var requesterId = HttpContext.GetRequesterId();
            await _mediator.Send(new DeletePlaylistCommand(requesterId, id), cancellationToken);
            return NoContent();
        }

        /// <summary>Adds a song to a playlist owned by the requester.</summary>
        [HttpPost("{id:guid}/songs")]
        public async Task<IActionResult> AddSong(
            Guid id,
            [FromBody] AddSongRequest body,
            CancellationToken cancellationToken)
        {
            var requesterId = HttpContext.GetRequesterId();
            await _mediator.Send(
                new AddSongToPlaylistCommand(requesterId, id, body.SongId), cancellationToken);
            return NoContent();
        }

        /// <summary>Removes a song from a playlist owned by the requester.</summary>
        [HttpDelete("{id:guid}/songs/{songId:guid}")]
        public async Task<IActionResult> RemoveSong(
            Guid id,
            Guid songId,
            CancellationToken cancellationToken)
        {
            var requesterId = HttpContext.GetRequesterId();
            await _mediator.Send(
                new RemoveSongFromPlaylistCommand(requesterId, id, songId), cancellationToken);
            return NoContent();
        }

        /// <summary>Adds a playlist to a target user's library.</summary>
        [HttpPost("{id:guid}/members")]
        public async Task<IActionResult> AddMember(
            Guid id,
            [FromBody] AddMemberRequest body,
            CancellationToken cancellationToken)
        {
            var requesterId = HttpContext.GetRequesterId();
            await _mediator.Send(
                new AddPlaylistToUserCommand(requesterId, body.UserId, id), cancellationToken);
            return NoContent();
        }

        /// <summary>Lists public playlists plus those owned by the requester.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 1,
            CancellationToken cancellationToken = default)
        {
            var requesterId = HttpContext.GetRequesterId();
            var result = await _mediator.Send(
                new GetAllPlaylistsQuery(requesterId, page, pageSize), cancellationToken);
            return Ok(result);
        }

        /// <summary>Lists songs in a playlist.</summary>
        [HttpGet("{id:guid}/songs")]
        public async Task<IActionResult> GetSongs(
            Guid id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 1,
            CancellationToken cancellationToken = default)
        {
            var requesterId = HttpContext.GetRequesterId();
            var result = await _mediator.Send(
                new GetPlaylistSongsQuery(requesterId, id, page, pageSize), cancellationToken);
            return Ok(result);
        }
    }
}