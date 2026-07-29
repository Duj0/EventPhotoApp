using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Pages
{
    [QueryProperty(nameof(PhotoUrl), "photoUrl")]
    public partial class FullScreenImage :ContentPage
    {
        public string PhotoUrl { get; set; }
       
        public FullScreenImage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (!string.IsNullOrEmpty(PhotoUrl))
                FullscreenImage.Source = ImageSource.FromUri(new Uri(PhotoUrl));
        }
        private async void OnDownloadClicked(object sender, EventArgs e)
        {
            try
            {
                var httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(PhotoUrl);
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
                await DisplayAlert("Saved", "Photo saved to gallery!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }
    }

}
