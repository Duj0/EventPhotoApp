using EventPhotoApp.Dtos;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventPhotoApp.Pages
{
    public partial class CreateEventPage : ContentPage
    {
        private readonly CreateEventApiService _api;
        public CreateEventPage(CreateEventApiService api) 
        {
            InitializeComponent();
            _api = api;
        }

        private async void OnSubmitEventClicked(object sender, EventArgs e)
        {
            var name = NameEntry.Text?.Trim();
            var date = DateEntry.Date.ToString("MM-dd-yyyy");
            var time = TimeEntry.Time.ToString(@"hh\:mm");
            var eventDateTime = DateEntry.Date + TimeEntry.Time;

            if (string.IsNullOrWhiteSpace(name)|| string.IsNullOrWhiteSpace(date)|| string.IsNullOrWhiteSpace(time))
            {
                await DisplayAlert("Error", "Fill all fields", "Ok");
                return;
            }

            try
            {
                var code = await _api.CreateEventAsync(new Dtos.CreateEventDto
                {
                    Name = name,
                    DateOfEvent = date,
                    TimeOfEvent = time,
                });
                await LocalNotificationCenter.Current.RequestNotificationPermission();
                var notification = new NotificationRequest
                {
                    NotificationId=1,
                    Title = "Event Starting soon",
                    Description = $"Your event '{name}' starts soon!",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = eventDateTime.AddMinutes(-15)
                    }
                };
                await LocalNotificationCenter.Current.Show(notification);
                await DisplayAlert("Success", $"Event Created! Join code: {code}", "Ok");

                await Navigation.PopAsync();

            }

            catch (Exception ex)
            {
                await DisplayAlert("Error",ex.ToString(),"Ok");
            }
        }
    }
}
