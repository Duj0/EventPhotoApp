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
            var image = sender as Image;
            var imageSource = image?.Source as UriImageSource;
            var url = imageSource?.Uri?.ToString();

            if (!string.IsNullOrEmpty(url))
                await Shell.Current.GoToAsync($"FullScreenImage?photoUrl={url}");
        }
    }
}
