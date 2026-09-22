using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlaylistControl.Application.Features.Songs.Queries.GetAllSongs;

namespace PlaylistControl.Api.Controllers
{
    /// <summary>
    /// Song endpoints.
    /// </summary>
    [ApiController]
    [Route("api/songs")]
    public class SongsController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SongsController"/> class.
        /// </summary>
        /// <param name="mediator">MediatR mediator</param>
        public SongsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Lists the static song catalog.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 1,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllSongsQuery(page, pageSize), cancellationToken);
            return Ok(result);
        }
    }
}