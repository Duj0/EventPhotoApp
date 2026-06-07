using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Pages
{
    public partial class SignInPageViewModel:ObservableObject
    {
        public SignInPageViewModel(FirebaseAuthClient authClient)
        {
            _authClient = authClient;
        }

        private readonly FirebaseAuthClient _authClient;
        [ObservableProperty]
        private string _email;
        [ObservableProperty]
        private string _password;

        [RelayCommand]
        private async Task SignIn() 
        {
            await _authClient.SignInWithEmailAndPasswordAsync(Email, Password);
            Preferences.Set("userEmail", Email);
            Preferences.Set("userPassword", Password);
            await Shell.Current.GoToAsync("//HomePage");
        }

        [RelayCommand]
        private async Task NavigateSignUp() 
        {
            await Shell.Current.GoToAsync("//SignUp");
        }
    }
}
