using PlaylistControl.Models;
using PlaylistControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlaylistControl.Controllers;

[ApiController]
[Route("[controller]")]
public class SongController : ControllerBase
{
    private readonly SongService _songService;
    private readonly PlaylistService _playlistService;

    public SongController(SongService songService, PlaylistService playlistService)
    {
        _songService = songService;
        _playlistService = playlistService;
    }

    // GET all action
    [HttpGet]
    public async Task<ActionResult<ICollection<Song>>> GetAll()
    {
        var songs = await _songService.GetAll();
        return Ok(songs);
    }

    // GET by Id action
    [HttpGet("{songId}")]
    public async Task<ActionResult<Song>> Get(int songId)
    {
        var song = await _songService.GetSongById(songId);
        if(song == null)
            return NotFound();
        return Ok(song);
    }

    // GET song details action
    [HttpGet("{songId}/details")]
    public async Task<ActionResult<string>> GetSongDetails(int songId)
    {
        var song = await _songService.GetSongById(songId);
        if(song == null)
            return NotFound();
        return Ok(_songService.GetSongDetails(song));
    }

    // Add song to playlist action
    [HttpPost("{songId}/addToPlaylist/{playlistId}")]
    public async Task<ActionResult> AddSongToPlaylist(int songId, int playlistId)
    {
        var song = await _songService.GetSongById(songId);
        var playlist = await _playlistService.GetPlaylistById(playlistId);

        if(song == null || playlist == null)
            return NotFound();

        await _playlistService.AddSongToPlaylist(playlistId, songId);
        return Ok();
    }
    
}
