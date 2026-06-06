using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Pages
{
    public partial class SignUpPageView:ContentPage
    {
        public SignUpPageView(SignUpPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
