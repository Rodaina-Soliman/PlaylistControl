using PlaylistControl.Models;
using PlaylistControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlaylistControl.Controllers;

[ApiController]
[Route("[controller]")]
public class PlaylistController : ControllerBase
{
    private readonly PlaylistService _playlistService;
    private readonly SongService _songService;

    public PlaylistController(PlaylistService playlistService, SongService songService)
    {
        _playlistService = playlistService;
        _songService = songService;
    }

    // GET all action
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var playlists = await _playlistService.GetAll();
        if (!playlists.Any()) return NotFound();
        return Ok(playlists);
    }

    //GET by id action
    [HttpGet("{playlistId}")]
    public async Task<ActionResult> Get(int playlistId)
    {
        var playlist = await _playlistService.GetPlaylistById(playlistId);
        if(playlist == null)
            return NotFound();
        return Ok(playlist);
    }

    // List songs in playlist action
    [HttpGet("{playlistId}/songs")]
    public async Task<ActionResult<List<Song>>> ListSongsInPlaylist(int playlistId)
    {
        var playlist = await _playlistService.GetPlaylistById(playlistId);
        if(playlist == null)
            return NotFound();

        var songs = await _playlistService.ListSongsInPlaylist(playlistId);
        return Ok(songs);
    }
    
}
