using EventPhotoApp.Models;
using EventPhotoApp.PageModels;

namespace EventPhotoApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}