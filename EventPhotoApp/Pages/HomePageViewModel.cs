using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    }
}
