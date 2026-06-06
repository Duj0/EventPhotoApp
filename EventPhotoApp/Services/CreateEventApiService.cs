using EventPhotoApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Services
{
    public class EventResponse 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TimeOfEvent { get; set; }
        public string DateOfEvent { get; set; }
        public string Code { get; set; }
    }
     public class CreateEventApiService
    {
        public readonly HttpClient _httpClient;

        public CreateEventApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateEventAsync(CreateEventDto request)
        {
            var response = await _httpClient.PostAsJsonAsync("/events", request);
            //System.Diagnostics.Debug.WriteLine("BASE: " + _httpClient.BaseAddress);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create event: {msg}");
            }
            var  result = await response.Content.ReadFromJsonAsync<EventResponse>();
            return result.Code;
        }

        public async Task<EventResponse> JoinEventAsync(string code) 
        {
            var response = await _httpClient.GetAsync($"/events/{code}");
            //System.Diagnostics.Debug.WriteLine("BASE: " + _httpClient.BaseAddress);
            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to join event: {msg}");
            }
            var result = await response.Content.ReadFromJsonAsync<EventResponse>();
            return result;
        }
    }
}
