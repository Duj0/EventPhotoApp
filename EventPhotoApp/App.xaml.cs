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
        protected override async void OnStart()
        {
            var email = Preferences.Get("userEmail", null);
            var pass = Preferences.Get("userPassword", null);

            System.Diagnostics.Debug.WriteLine($"Email from prefs: {email ?? "null"}");

            if (email != null && pass != null)
            {
                await _firebaseAuth.SignInWithEmailAndPasswordAsync(email, pass);
                await Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
                await Shell.Current.GoToAsync("//SignIn");
            }
        }
    }
}