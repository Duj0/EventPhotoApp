using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Pages
{
    public partial class SignUpPageViewModel :ObservableObject
    {
        public SignUpPageViewModel(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        private readonly FirebaseAuthClient _authClient;
        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [RelayCommand]
        private async Task SignUp()
        {
            try
            {
                await _authClient.CreateUserWithEmailAndPasswordAsync(Email, Password, Username);
                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }

        [RelayCommand]
        private async Task NavigateSignIn()
        {
            await Shell.Current.GoToAsync("//SignIn");
        }

    }
}
