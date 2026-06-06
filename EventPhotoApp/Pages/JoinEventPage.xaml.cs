using EventPhotoApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EventPhotoApp.Pages
{
    public partial class JoinEventPage:ContentPage
    {

        private readonly CreateEventApiService _api;

        public JoinEventPage(CreateEventApiService api)
        {
            InitializeComponent();
            _api = api;
        }

        private async void OnSubmitEventClicked(object sender, EventArgs e) 
        {
            var code  = CodeEntry.Text;
            if (string.IsNullOrWhiteSpace(code))
            {
                await DisplayAlert("Error", "Fill all fields", "Ok");
                return;
            }
            try
            {
                var join = await _api.JoinEventAsync(code);
                await DisplayAlert("Success", $"You have joined the event of: {join.Name} with code:{join.Code}", "Ok");
                Preferences.Set("eventId", join.Id.ToString());
                await Shell.Current.GoToAsync($"PhotosPage?eventId={join.Id}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }


    }
}
