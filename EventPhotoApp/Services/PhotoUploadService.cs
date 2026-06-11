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
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", keys.SupabaseKey);
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
