using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
namespace EventPhotoApp
{
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
    Categories = new[] {
        Android.Content.Intent.CategoryDefault,
        Android.Content.Intent.CategoryBrowsable
    },
    DataScheme = "eventsnap")]

    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#1A1A2E"));
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            var data = intent?.Data;
            if (data != null)  
            {
                var code = data.GetQueryParameter("code");
                if (!string.IsNullOrEmpty(code))
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(500);
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await Shell.Current.GoToAsync($"//JoinEvent?code={code}");
                        });
                    });
                }
            }
        }
    }
}