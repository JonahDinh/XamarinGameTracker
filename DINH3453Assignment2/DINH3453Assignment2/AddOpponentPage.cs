using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Xamarin.Forms;

namespace DINH3453Assignment2
{
    // Page for adding a new opponent

    //Using an action to act as a callback
    //Had to use external documentation to learn:
    //https://learn.microsoft.com/en-us/xamarin/essentials/app-actions?tabs=android

    public class AddOpponentPage : ContentPage
    {
        //Using an action to 
        //https://learn.microsoft.com/en-us/xamarin/essentials/app-actions?tabs=android

        private Action refreshCallback;
        // Constructor
        public AddOpponentPage(Action refreshCallback)
        {
            Title = "Add New Opponent";
            this.refreshCallback = refreshCallback;

            // Entry fields for opponent details
            Entry firstNameEntry = new Entry { Placeholder = "First Name" };
            Entry lastNameEntry = new Entry { Placeholder = "Last Name" };
            Entry addressEntry = new Entry { Placeholder = "Address" };
            Entry phoneEntry = new Entry { Placeholder = "Phone" };
            Entry emailEntry = new Entry { Placeholder = "Email" };
            Label firstNameLabel = new Label { Text = "First Name:", VerticalOptions = LayoutOptions.Center, WidthRequest = 100 };
            Label lastNameLabel = new Label { Text = "Last Name:", VerticalOptions = LayoutOptions.Center, WidthRequest = 100 };
            Label addressLabel = new Label { Text = "Address:", VerticalOptions = LayoutOptions.Center, WidthRequest = 100 };
            Label phoneLabel = new Label { Text = "Phone:", VerticalOptions = LayoutOptions.Center, WidthRequest = 100 };
            Label emailLabel = new Label { Text = "Email:", VerticalOptions = LayoutOptions.Center, WidthRequest = 100 };

            // Grid to arrange labels and entry fields
            Grid grid = new Grid();
            //https://learn.microsoft.com/en-us/xamarin/android/user-interface/layouts/grid-layout
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.Margin = 20;

            grid.Children.Add(firstNameLabel, 0, 0);
            grid.Children.Add(firstNameEntry, 1, 0);
            grid.Children.Add(lastNameLabel, 0, 1);
            grid.Children.Add(lastNameEntry, 1, 1);
            grid.Children.Add(addressLabel, 0, 2);
            grid.Children.Add(addressEntry, 1, 2);
            grid.Children.Add(phoneLabel, 0, 3);
            grid.Children.Add(phoneEntry, 1, 3);
            grid.Children.Add(emailLabel, 0, 4);
            grid.Children.Add(emailEntry, 1, 4);

            // Error label for displaying validation errors
            Label errorLabel = new Label
            {
                TextColor = Color.Red,
                FontAttributes = FontAttributes.Italic,
                IsVisible = false
            };

            // Button for saving opponent details
            Button saveButton = new Button { Text = "Save", Margin = new Thickness(20, 10, 20, 20) };
            saveButton.Clicked += (sender, e) =>
            {
                string firstName = firstNameEntry.Text;
                string lastName = lastNameEntry.Text;
                string address = addressEntry.Text;
                string phone = phoneEntry.Text;
                string email = emailEntry.Text;

                // Check if any field is empty
                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                    string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(phone) ||
                    string.IsNullOrWhiteSpace(email))
                {
                    errorLabel.Text = "Please fill in all fields.";
                    errorLabel.IsVisible = true;
                    return;
                }

                errorLabel.IsVisible = false;

                // Create new opponent object and save to database
                Opponents newOpponent = new Opponents
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Address = address,
                    Phone = phone,
                    Email = email
                };

                App.Database.SaveOpponent(newOpponent);

                Navigation.PopAsync(); // Navigate back after saving
                refreshCallback?.Invoke(); //Callback function. Have to invoke it.
            };

            StackLayout layout = new StackLayout();
            layout.Children.Add(grid);
            layout.Children.Add(errorLabel); // Add error label to the layout
            layout.Children.Add(saveButton);

            Content = layout;
        }
    }
}
