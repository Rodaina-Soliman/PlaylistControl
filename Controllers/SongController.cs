using PlaylistControl.Models;
using PlaylistControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlaylistControl.Controllers;

[ApiController]
[Route("[controller]")]
public class SongController : ControllerBase
{
    public SongController()
    {
    }

    // GET all action
    [HttpGet]
    public ActionResult<ICollection<Song>> GetAll() =>
        SongService.GetAll();

    // GET by Id action
    [HttpGet("{songId}")]
    public ActionResult<Song> Get(int songId)
    {
        var song = SongService.GetAll().FirstOrDefault(s => s.Id == songId);

        if(song == null)
            return NotFound();

        return song;
    }

    // GET song details action
    [HttpGet("{songId}/details")]
    public ActionResult<string> GetSongDetails(int songId)
    {
        var song = SongService.GetAll().FirstOrDefault(s => s.Id == songId);

        if(song == null)
            return NotFound();

        return SongService.GetSongDetails(song);
    }

    // Add song to playlist action
    [HttpPost("{songId}/addToPlaylist/{playlistId}")]
    public ActionResult AddSongToPlaylist(int songId, int playlistId)
    {
        var song = SongService.GetSongById(songId);
        var playlist = PlaylistService.GetPlaylistById(playlistId);

        if(song == null || playlist == null)
            return NotFound();

        PlaylistService.AddSongToPlaylist(playlist, song);
        return Ok();
    }
    
}
