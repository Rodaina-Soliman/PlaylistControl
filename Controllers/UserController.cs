using PlaylistControl.Models;
using PlaylistControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlaylistControl.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public UserController()
    {
    }

    // GET all action
    [HttpGet]
    public ActionResult<ICollection<User>> GetAll() =>
        UserService.GetAll();

    // GET by Id action
    [HttpGet("{userId}")]
    public ActionResult<User> Get(int userId)
    {
        var user = UserService.getUserById(userId);

        if(user == null)
            return NotFound();

        return user;
    }

    // GET playlists for user action
    [HttpGet("{userId}/playlists")]
    public ActionResult<ICollection<Playlist>> GetPlaylistsForUser(int userId)
    {
        var user = UserService.getUserById(userId);

        if(user == null)
            return NotFound();

        return Ok(UserService.ListPlaylistsForUser(user));
    }

    // POST create playlist for user action
    [HttpPost("{userId}/playlists")]
    public ActionResult CreatePlaylistForUser(int userId, [FromBody] Playlist playlist)
    {
        var user = UserService.getUserById(userId);

        if(user == null)
            return NotFound();

        UserService.CreatePlaylistForUser(user, playlist.Id, playlist.Name, playlist.Description);
        return Ok();
    }

    // DELETE remove playlist from user action
    [HttpDelete("{userId}/playlists/{playlistId}")]
    public ActionResult RemovePlaylistFromUser(int userId, int playlistId)
    {
        var user = UserService.getUserById(userId);

        if(user == null)
            return NotFound();

        var playlist = user.Playlists?.FirstOrDefault(p => p.Id == playlistId);
        if(playlist == null)
            return NotFound();

        UserService.RemovePlaylistFromUser(user, playlist);
        return Ok();
    }


    
}
