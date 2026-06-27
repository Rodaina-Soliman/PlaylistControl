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
    private PlaylistService _playlistService;
    private SongService _songService;
    public UserController(UserService userService, SongService songService, PlaylistService playlistService)
    {
        _userService = userService;
        _playlistService = playlistService;
        _songService = songService;
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

    // POST add a playlist to user action
    [HttpPost("{userId}/playlists/{playlistId}")]
    public async Task<ActionResult> AddPlaylistForUser(int userId, int playlistId)
    {
        var user = await _userService.GetUserById(userId);
        var playlist = await _playlistService.GetPlaylistById(playlistId);
        if(user == null || playlist == null)
            return NotFound();
        await _userService.AddPlaylistForUser(userId, playlistId);
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

    // Add song to playlist action
    [HttpPost("{userId}/songs/{songId}/addToPlaylist/{playlistId}")]
    public async Task<ActionResult> AddSongToPlaylist(int songId, int playlistId, int userId)
    {
        var song = await _songService.GetSongById(songId);
        var playlist = await _playlistService.GetPlaylistById(playlistId);
        var belongsToUser = await _userService.PlaylistBelongsToUser(playlistId, userId);

        if(song == null || playlist == null || !belongsToUser)
            return NotFound();

        await _playlistService.AddSongToPlaylist(playlistId, songId);
        return Ok();
    }

    // Remove song from playlist action
    [HttpDelete("{userId}/playlists/{playlistId}/RemoveFromPlaylist/{songId}")]
    public async Task<ActionResult> RemoveSongFromPlaylist(int playlistId, int songId, int userId)
    {
        var playlist = await _playlistService.GetPlaylistById(playlistId);
        var song = await _songService.GetSongById(songId);
        var belongsToUser = await _userService.PlaylistBelongsToUser(playlistId, userId);

        if(playlist == null || song == null || !belongsToUser)
            return NotFound();

        await _playlistService.RemoveSongFromPlaylist(playlistId, songId);
        return Ok();
    }

    // Add user to database
    [HttpPost]
    public async Task<ActionResult> AddUser([FromBody] User user)
    {
        await _userService.Add(user);
        return Ok();
    }

    //Delete user from database
    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeleteSong(int userId)
    {
        var user = await _userService.GetUserById(userId);
        if (user == null)
        {
            return NotFound();
        }
        await _userService.Delete(userId);
        return Ok();
    }

}
