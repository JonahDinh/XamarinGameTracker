using System;
using Xamarin.Forms;

namespace DINH3453Assignment2
{
    public class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            Title = "Settings";

            //Button to reset
            Button resetButton = new Button
            {
                Text = "Reset App",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                IsEnabled = true
            };

            resetButton.Clicked += (sender, e) =>
            {
                App.Database.ResetApp();
                //Disabling the button after click
                resetButton.IsEnabled = false;
                
            };
            Content = new StackLayout
            {
                Children = {
                    resetButton
                }
            };
        }
    }
}
