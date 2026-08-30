using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Services
{
    public class TokenService
    {
        public readonly HttpClient _httpClient;
        public TokenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task RegisterTokenAsync(string token, string eventId, string role)
        {
            var response = await _httpClient.PostAsJsonAsync("/tokens", new {  token,  eventId,  role });

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create event: {msg}");
            }
        }   
    }
}