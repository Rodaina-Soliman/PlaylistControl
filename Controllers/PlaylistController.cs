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

    // Remove song from playlist action
    [HttpDelete("{playlistId}/removeSong/{songId}")]
    public async Task<ActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
    {
        var playlist = await _playlistService.GetPlaylistById(playlistId);
        var song = await _songService.GetSongById(songId);

        if(playlist == null || song == null)
            return NotFound();

        await _playlistService.RemoveSongFromPlaylist(playlistId, songId);
        return Ok();
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
