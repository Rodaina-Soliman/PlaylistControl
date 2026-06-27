using PlaylistControl.Models;
using PlaylistControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlaylistControl.Controllers;

[ApiController]
[Route("[controller]")]
public class PlaylistController : ControllerBase
{
    public PlaylistController()
    {
    }

    // Remove song from playlist action
    [HttpDelete("{playlistId}/removeSong/{songId}")]
    public ActionResult RemoveSongFromPlaylist(int playlistId, int songId)
    {
        var playlist = PlaylistService.GetPlaylistById(playlistId);
        var song = SongService.GetSongById(songId);

        if(playlist == null || song == null)
            return NotFound();

        PlaylistService.RemoveSongFromPlaylist(playlist, song);
        return Ok();
    }

    // List songs in playlist action
    [HttpGet("{playlistId}/songs")]
    public ActionResult ListSongsInPlaylist(int playlistId)
    {
        var playlist = PlaylistService.GetPlaylistById(playlistId);

        if(playlist == null)
            return NotFound();

        PlaylistService.ListSongsInPlaylist(playlist);
        return Ok();
    }
    
}
