namespace EventPhotoApp

{
    using EventPhotoApp.Pages;
    using Firebase.Auth;

    public partial class App : Application
    {
        private readonly FirebaseAuthClient _firebaseAuth;
        public App(AppShell appShell, FirebaseAuthClient firebaseAuth)
        {
            _firebaseAuth = firebaseAuth;
            InitializeComponent();
            MainPage = appShell;
            


            
        }
        protected override void OnStart()
        {
            System.Diagnostics.Debug.WriteLine(_firebaseAuth.User?.Uid ?? "null");
            if (_firebaseAuth.User != null)
            {
                Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
            Shell.Current.GoToAsync("//SignIn");
            }
        }
    }
}