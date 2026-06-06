using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventPhotoApp.Services
{
    public class PhotoResponse
    {
        public string eventId { get; set; }

        [JsonPropertyName("photo_url")]
        public string PhotoUrl { get; set; }
        public string UploadedBy { get; set; }
    }
    public class SavePhotoService
    {
        
        public readonly HttpClient _httpClient;

        public SavePhotoService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

            public async Task<string> SavePhoto(string eventId, string photoUrl, string uploadedBy)
            {
                var response = await _httpClient.PostAsJsonAsync("/photos", new { eventId, photoUrl, uploadedBy });
                if (!response.IsSuccessStatusCode) 
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create event: {msg}");
                }

                var result = await response.Content.ReadFromJsonAsync<PhotoResponse>();
                return result.PhotoUrl;
            }

        public async Task<List<string>> GetPhotoAsync(string eventId) 
        {
            var response = await _httpClient.GetAsync($"photos/{eventId}");

            if (!response.IsSuccessStatusCode) 
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to get photos: {msg}");
            }
            var result = await response.Content.ReadFromJsonAsync<List<PhotoResponse>>();
            return result.Select(p => p.PhotoUrl).ToList();
        }
    }
}
