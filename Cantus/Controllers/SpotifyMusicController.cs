using Cantus.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Cantus.Models;

namespace Cantus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyMusicController : ControllerBase
    {
        private readonly SpotifyService _spotifyService;

        public SpotifyMusicController(SpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query, string type)
        {
            var response = await _spotifyService.SearchAsync(query, type);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }

        [HttpGet("{type}/{id}")]
        public async Task<IActionResult> GetItem(string type, string id)
        {
            var response = await _spotifyService.GetItemAsync(id, type);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }

        [HttpPost("playlists")]
        public async Task<IActionResult> CreatePlaylist([FromBody] Playlist playlist)
        {
            var response = await _spotifyService.CreatePlaylistAsync(playlist);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Ok(content);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }

        [HttpPost("playlists/{playlistId}/tracks")]
        public async Task<IActionResult> AddTrackToPlaylist(string playlistId, string trackUri)
        {
            var response = await _spotifyService.AddTrackToPlaylistAsync(playlistId, trackUri);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }

        [HttpDelete("playlists/{playlistId}/tracks")]
        public async Task<IActionResult> RemoveTrackFromPlaylist(string playlistId, string trackUri)
        {
            var response = await _spotifyService.RemoveTrackFromPlaylistAsync(playlistId, trackUri);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }
    }
}
