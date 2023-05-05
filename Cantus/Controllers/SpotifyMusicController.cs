using Cantus.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Cantus.Models;
using System.Text;

namespace Cantus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyMusicController : ControllerBase
    {
        private readonly SpotifyService _spotifyService;
        private readonly IConfiguration _configuration;

        public SpotifyMusicController(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;

            // Retrieve access token from request headers
            var accessToken = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Create new instance of SpotifyService with HttpClient and access token
            _spotifyService = new SpotifyService(httpClient, accessToken);
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

        [HttpPost("refresh_token")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var clientId = _configuration["Spotify:ClientId"];
            var clientSecret = _configuration["Spotify:ClientSecret"];

            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        { "grant_type", "refresh_token" },
        { "refresh_token", refreshToken }
    });

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Headers =
        {
            { "Authorization", $"Basic {authHeader}" }
        },
                Content = content
            };

            var client = new HttpClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<SpotifyTokenResponse>(responseContent);

                return Ok(new { access_token = tokenResponse.AccessToken });
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }
    }
}
