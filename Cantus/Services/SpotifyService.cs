using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Cantus.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Cantus.Services
{
    public class SpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessToken;

        public SpotifyService(HttpClient httpClient, string accessToken)
        {
            _httpClient = httpClient;
            _accessToken = accessToken;

            _httpClient.BaseAddress = new Uri("https://api.spotify.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
      

        //Search
        public async Task<HttpResponseMessage> SearchAsync(string query, string type)
        {
            var response = await _httpClient.GetAsync($"search?q={query}&type={type}");
            return response;
        }

        //Get Track,Album or Artist
        public async Task<HttpResponseMessage> GetItemAsync(string id, string type)
        {
            var response = await _httpClient.GetAsync($"{type}s/{id}");
            return response;
        }

        //Create Playlist
        public async Task<HttpResponseMessage> CreatePlaylistAsync(Playlist playlist)
        {
            var json = JsonConvert.SerializeObject(playlist);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("users/{user_id}/playlists", content);
            return response;
        }

        //Adding track from playlist
        public async Task<HttpResponseMessage> AddTrackToPlaylistAsync(string playlistId, string trackUri)
        {
            var json = JsonConvert.SerializeObject(new { uris = new[] { trackUri } });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"playlists/{playlistId}/tracks", content);
            return response;
        }

        //Removing track from playlist
        public async Task<HttpResponseMessage> RemoveTrackFromPlaylistAsync(string playlistId, string trackUri)
        {
            var json = JsonConvert.SerializeObject(new { tracks = new[] { new { uri = trackUri } } });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Delete, $"playlists/{playlistId}/tracks")
            {
                Content = content
            };
            var response = await _httpClient.SendAsync(request);
            return response;
        }

        //Playing track
        public async Task<HttpResponseMessage> PlayAsync(string deviceId, string contextUri, int? offset = null)
        {
            var json = JsonConvert.SerializeObject(new { context_uri = contextUri, offset = new { position = offset } });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"me/player/play?device_id={deviceId}", content);
            return response;
        }
    }

}
