using EventPhotoApp.Dtos;
using Microsoft.Extensions.Logging;
using Plugin.Firebase.CloudMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EventPhotoApp.Pages
{
    [QueryProperty(nameof(Code), "code")]

    public partial class JoinEventPage:ContentPage
    {
        public string Code
        {
            get; set;
        }

        private readonly CreateEventApiService _api;
        private readonly TokenService _tokenService;
        public JoinEventPage(CreateEventApiService api, TokenService tokenService)
        {
            InitializeComponent();
            _api = api;
            _tokenService = tokenService;
        }


        private async void OnSubmitEventClicked(object sender, EventArgs e) 
        {
            var code = Code;
            if (string.IsNullOrWhiteSpace(code)) 
            {
                 code  = CodeEntry.Text;
            }
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
                var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                await _tokenService.RegisterTokenAsync(token, join.Id.ToString(), "guest");
                await Shell.Current.GoToAsync($"PhotosPage?eventId={join.Id}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }


    }
}
