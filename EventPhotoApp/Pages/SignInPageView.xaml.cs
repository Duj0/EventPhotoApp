using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Pages
{
    public partial class SignInPageView : ContentPage
    {
        readonly FirebaseAuthClient? authClient;

        public SignInPageView(SignInPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

       
    }
}