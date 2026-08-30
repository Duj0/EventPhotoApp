using CommunityToolkit.Maui;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.CloudMessaging;
using Plugin.LocalNotification;
using Syncfusion.Maui.Toolkit.Hosting;
#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

namespace EventPhotoApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .RegisterFirebaseServices()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .UseLocalNotification()
                .ConfigureMauiHandlers(handlers =>
                {
#if IOS || MACCATALYST
    				handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });
#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif
            builder.Services.AddSingleton<ModalErrorHandler>();
            builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = keys.FirebaseApiKey,
                AuthDomain = "eventphotsharingapp.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            }));
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddTransient<CreateEventPage>();
            builder.Services.AddTransient<JoinEventPage>();
            builder.Services.AddSingleton<HomePageViewModel>();
            builder.Services.AddSingleton<SignInPageView>();
            builder.Services.AddSingleton<SignInPageViewModel>();
            builder.Services.AddSingleton<SignUpPageView>();
            builder.Services.AddSingleton<SignUpPageViewModel>();
            builder.Services.AddTransientWithShellRoute<PhotosPage, PhotosPage>("PhotosPage");
            builder.Services.AddTransientWithShellRoute<FullScreenImage, FullScreenImage>("FullScreenImage");
            builder.Services.AddSingleton(x => CrossFirebaseCloudMessaging.Current);
            builder.Services.AddSingleton(sp =>
            new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5189/") });
            builder.Services.AddSingleton<CreateEventApiService>();
            builder.Services.AddTransient<PhotoUploadService>();
            builder.Services.AddSingleton<SavePhotoService>();
            builder.Services.AddSingleton<TokenService>();
            return builder.Build();
        }

        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) => CrossFirebase.Initialize(activity, () => Platform.CurrentActivity)));
#endif
            });
            return builder;
        }
    }
}