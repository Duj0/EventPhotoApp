using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System;
using System.IO.Compression;
using System.Threading.Tasks;

namespace EventPhotoApp.Pages
{

    [QueryProperty(nameof(EventId), "eventId")]
    public partial class PhotosPage : ContentPage
    {
        public string EventId
        {
            get; set;
        }
        private System.Threading.PeriodicTimer? _timer;

        private readonly PhotoUploadService _api;
        private readonly SavePhotoService _savePhotoService;

        public PhotosPage(PhotoUploadService api, SavePhotoService savePhotoService)
        {
            InitializeComponent();
            _api = api;
            _savePhotoService = savePhotoService;
        }
        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null)
            {
                return;
            }
            try
            {
                var url = await _api.UploadPhoto(photo);
                await DisplayAlert("Uploaded","Photo was successfully uploaded", "OK");
                var savePhoto = await _savePhotoService.SavePhoto(EventId, url, "Guest");
                var photos = await _savePhotoService.GetPhotoAsync(EventId);
                PhotosCollection.ItemsSource = photos;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Ok");
                throw;
            }
        }
        private async void OnPickPhotoClicked(object sender, EventArgs e)
        {
            var photos = await FilePicker.Default.PickMultipleAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[]{ "image/*", "video/*"} },
                    { DevicePlatform.iOS, new[]{ "public.image", "public.movie"} }
                }),
                PickerTitle = "Select photos and videos"
            });
            if (photos == null)
            {
                return;
            }
            try
            {
                foreach (var photo in photos) 
                {
                    var url = await _api.UploadPhoto(photo);
                    var savePhoto = await _savePhotoService.SavePhoto(EventId, url, "Guest");
                }
                await DisplayAlert("Uploaded", "Photo was successfully uploaded", "OK");
                var upadatedPhotos = await _savePhotoService.GetPhotoAsync(EventId);
                PhotosCollection.ItemsSource = photos;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Ok");
                throw;
            }
        }
        
        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            try
            {
                base.OnNavigatedTo(args);
                EventId ??= Preferences.Get("eventId", string.Empty);

                if (string.IsNullOrEmpty(EventId))
                {
                    await DisplayAlert("Error", "Event ID is missing. Join an event", "OK");
                    return;
                }

                var code = await _savePhotoService.GetEventCodeAsync(EventId);
                CodeLabel.Text = $"Event Code: {code}";
                var photos = await _savePhotoService.GetPhotoAsync(EventId);
                PhotosCollection.ItemsSource = photos;
                if (_timer==null)
                {
                    var timer = new System.Threading.PeriodicTimer(TimeSpan.FromSeconds(5)); _ = Task.Run(async () =>
                    {
                        while (await timer.WaitForNextTickAsync())
                        {
                            var newPhotos = await _savePhotoService.GetPhotoAsync(EventId);
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                PhotosCollection.ItemsSource = newPhotos;
                            });
                        }
                    });
                }
                
            }
            catch (Exception ex)
            {

                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }

        private async void OnPhotoTapped(object sender, TappedEventArgs e)
        {
            var image = sender as Image;
            var imageSource = image?.Source as UriImageSource;
            var url = imageSource?.Uri?.ToString();

            if (!string.IsNullOrEmpty(url))
                await Shell.Current.GoToAsync($"FullScreenImage?photoUrl={url}");
        }

        private async void OnLeaveEventClicked(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Leave the event", "Do you want to leave the event?", "Yes", "No");
            if (result)
            {
                Preferences.Remove("eventId");
                await Shell.Current.GoToAsync("//HomePage");
            }
        }

        private async void OnDownloadAsZipClicked(object sender, EventArgs e)
        {
            var data = PhotosCollection.ItemsSource as List<string>;
            if (data == null || data.Count == 0)
            {
                await DisplayAlert("Error", "No photos to export yet", "OK");
                return;
            }

            var filePath = Path.Combine(FileSystem.CacheDirectory, "EventPhotos.zip");

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
            {
                var httpClient = new HttpClient();
                int photosCount = 1;
                foreach (var photoUrl in data)
                {
                    var entry = archive.CreateEntry($"photo_{photosCount}.jpg");
                    using var entryStream = entry.Open();
                    using var downloadStream = await httpClient.GetStreamAsync(photoUrl);
                    await downloadStream.CopyToAsync(entryStream);
                    photosCount++;
                }
            }

            using var savedFileStream = File.OpenRead(filePath);
            var result = await FileSaver.Default.SaveAsync("EventPhotos.zip", savedFileStream);

            if (result != null)
            {
                await DisplayAlert("Success", "Photos exported as ZIP file!", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to save ZIP file.", "OK");
            }

            File.Delete(filePath);
        }

        private async void OnNotifyGuestsClicked(object sender, EventArgs e)
        {
            try
            {
                
                await _savePhotoService.SendNotification(EventId, "Don't forget to download your photos before the event ends!");
                await DisplayAlert("Sent", "Guests have been notified!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
        }
    }
}
