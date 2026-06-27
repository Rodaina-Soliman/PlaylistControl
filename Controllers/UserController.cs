using PlaylistControl.Models;
using PlaylistControl.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PlaylistControl.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    // GET all action
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAll();
        if (!users.Any()) return NotFound();
        return Ok(users);
    }

    // GET by Id action
    [HttpGet("{userId}")]
    public async Task<ActionResult> Get(int userId)
    {
        var user = await _userService.GetUserById(userId);

        if(user == null)
            return NotFound();

        return Ok(user);
    }

    // GET playlists for user action
    [HttpGet("{userId}/playlists")]
    public async Task<ActionResult<ICollection<Playlist>>> GetPlaylistsForUser(int userId)
    {
        var user = await _userService.GetUserById(userId);
        if(user == null)
            return NotFound();

        var playlists = await _userService.ListPlaylistsForUser(userId);
        return Ok(playlists);
    }

    // POST create playlist for user action
    [HttpPost("{userId}/playlists")]
    public async Task<ActionResult> CreatePlaylistForUser(int userId, [FromBody] Playlist playlist)
    {
        var user = await _userService.GetUserById(userId);
        if(user == null)
            return NotFound();

        await _userService.CreatePlaylistForUser(user, playlist);
        return Ok();
    }

    // DELETE remove playlist from user action
    [HttpDelete("{userId}/playlists/{playlistId}")]
    public async Task<ActionResult> RemovePlaylistFromUser(int userId, int playlistId)
    {
        var user = await _userService.GetUserById(userId);
        if(user == null)
            return NotFound();

        var playlists = await _userService.ListPlaylistsForUser(userId);
        var playlist = playlists?.FirstOrDefault(p => p.Id == playlistId);
        if(playlist == null)
            return NotFound();

        await _userService.RemovePlaylistFromUser(userId, playlistId);
        return Ok();
    }

}
