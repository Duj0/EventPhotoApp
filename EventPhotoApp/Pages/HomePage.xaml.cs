using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Microsoft.Maui.Controls;
using System;

namespace EventPhotoApp.Pages
{
    public partial class HomePage : ContentPage
    {

        public HomePage(HomePageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
           
        }

       
    }
}