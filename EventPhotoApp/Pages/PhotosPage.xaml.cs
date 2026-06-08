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
                await DisplayAlert("Error", "Fill all fields", "Ok");
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

                using var stream = new MemoryStream(bytes);
                var result = await FileSaver.Default.SaveAsync(fileName, stream);

                if (result.IsSuccessful)
                    await DisplayAlert("Saved", "Photo saved successfully!", "OK");
            }
            catch (Exception ex)
            {

                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }


    }
}
