using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Services
{
    public class PhotoUploadService
    {
        public readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://wqufmlkobqtxebnpgoer.supabase.co/")
        };

        public PhotoUploadService()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6IndxdWZtbGtvYnF0eGVibnBnb2VyIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc3OTIyMjMxMCwiZXhwIjoyMDk0Nzk4MzEwfQ.flbt8XY-2j6bo1AUknnnjuPVDLJ3JlUhXF1A5mylBXQ");
        }


        public async Task<string> UploadPhoto(FileResult photo) 
        {
            var stream = await photo.OpenReadAsync();
            var result = await _httpClient.PostAsync($"/storage/v1/object/photos/{photo.FileName}", new StreamContent(stream));
                if (!result.IsSuccessStatusCode)
                {
                    var msg = await result.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to upload photo: {msg}");
                }
                else
                {
                    var url = $"https://wqufmlkobqtxebnpgoer.supabase.co/storage/v1/object/public/photos/{photo.FileName}";
                    return url;
                }
        }

    }
}
