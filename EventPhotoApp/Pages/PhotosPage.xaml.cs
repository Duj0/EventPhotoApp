using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System;
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
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo == null)
            {
                return;
            }
            try
            {
                var url = await _api.UploadPhoto(photo);
                await DisplayAlert("Uploaded", "Photo was successfully uploaded", "OK");
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
            try
            {
                var image = sender as Image;
                var imageSource = image?.Source as UriImageSource;
                var url = imageSource?.Uri?.ToString();
                var httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(url);
                var fileName = $"photo_{DateTime.Now:yyyyMMddHHmmss}.jpg";

#if ANDROID
                var contentValues = new Android.Content.ContentValues();
                contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, fileName);
                contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "image/jpeg");
                contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryPictures + "/EventPhotos");
                var uri = Android.App.Application.Context.ContentResolver.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri, contentValues);

                using var stream = Android.App.Application.Context.ContentResolver.OpenOutputStream(uri);
                await stream.WriteAsync(bytes);
#endif
                await DisplayAlert("Saved", "Photo saved successfully!", "OK");

            }
            catch (Exception ex)
            {

                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }
    }
}
