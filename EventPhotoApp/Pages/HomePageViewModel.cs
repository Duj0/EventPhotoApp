using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Pages
{
    public partial class HomePageViewModel: ObservableObject
    {
        private readonly FirebaseAuthClient _authClient;
        public HomePageViewModel(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        [RelayCommand]
        private async Task CreateEvent()
        {
            await Shell.Current.GoToAsync("//CreateEvent");
        }

        [RelayCommand]
        private async Task PhotosPage()
        {
            await Shell.Current.GoToAsync($"PhotosPage");
        }

        [RelayCommand]
        private async Task JoinEvent()
        {
            await Shell.Current.GoToAsync("//JoinEvent");
        }
        [RelayCommand]
        private async Task SignOut()
        {
            try
            {
                _authClient.SignOut();
                Preferences.Remove("userEmail");
                Preferences.Remove("userPassword");
                Preferences.Remove("eventId");
                await Shell.Current.GoToAsync("//SignIn");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "OK");
            }
        }
    }
}
