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
        if (!songs.Any()) return NotFound();
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

    // Add song to database
    [HttpPost]
    public async Task<ActionResult> AddSong([FromBody] Song song)
    {
        await _songService.AddSong(song);
        return Ok();
    }

    //Delete song from database
    [HttpDelete("{songId}")]
    public async Task<ActionResult> DeleteSong(int songId)
    {
        var song = await _songService.GetSongById(songId);
        if (song == null)
        {
            return NotFound();
        }
        await _songService.DeleteSong(songId);
        return Ok();
    }

    // PUT update song action
    [HttpPut("{songId}")]
    public async Task<ActionResult> UpdateSong(int songId, [FromBody] Song updatedSong)
    {
        var existingSong = await _songService.GetSongById(songId);
        if (existingSong == null)
            return NotFound();

        await _songService.UpdateSong(songId, updatedSong);
        return Ok();
    }

}
