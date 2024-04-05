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
    public class OpponentsListPage : ContentPage
    {
        private ObservableCollection<Opponents> opponentsList;

        public OpponentsListPage()
        {
            Title = "Opponents";

            //Getting a list of ALL opponents from database
            opponentsList = new ObservableCollection<Opponents>(App.Database.GetOpponents());

            var opponentsListView = new ListView
            {
                ItemsSource = opponentsList,
                ItemTemplate = new DataTemplate(typeof(OpponentCell)),
                RowHeight = OpponentCell.RowHeight,
                IsPullToRefreshEnabled = true
            };

            //OnClick for the MatchPage
            opponentsListView.ItemTapped += (sender, e) =>
            {
                opponentsListView.SelectedItem = null;
                Navigation.PushAsync(new MatchesPage(e.Item as Opponents));
            };

            StackLayout layout = new StackLayout();
            layout.Children.Add(opponentsListView);

            Button btnNew = new Button { Text = "Add New Opponent" };
            btnNew.Clicked += (sender, e) =>
            {
                //https://learn.microsoft.com/en-us/xamarin/essentials/app-actions?tabs=android
                //Needed to use a callback function so that the page refreshes.
                Navigation.PushAsync(new AddOpponentPage(RefreshOpponentList));
            };
            layout.Children.Add(btnNew);

            //Swipe to refresh, not really needed but nice to have
            opponentsListView.Refreshing += async (sender, e) =>
            {
                RefreshOpponentList();
                opponentsListView.EndRefresh();
            };

            Content = layout;
        }
        /// <summary>
        /// Helper method to refresh the list. This clears the current list and re-adds from the database.
        /// </summary>
        private void RefreshOpponentList()
        {
            var updatedOpponents = App.Database.GetOpponents();

            opponentsList.Clear();
            foreach (var opponent in updatedOpponents)
            {
                opponentsList.Add(opponent);
            }
        }

    }
    /// <summary>
    /// View cell for the list.
    /// </summary>
    public class OpponentCell : ViewCell
    {
        public const int RowHeight = 55;

        public OpponentCell()
        {
            Label lblFirstName = new Label { FontAttributes = FontAttributes.Bold };
            lblFirstName.SetBinding(Label.TextProperty, "FirstName");

            Label lblLastName = new Label { FontAttributes = FontAttributes.Bold };
            lblLastName.SetBinding(Label.TextProperty, "LastName");

            Label lblPhone = new Label();
            lblPhone.SetBinding(Label.TextProperty, "Phone");

            Grid grid = new Grid
            {
                Padding = 10,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            grid.Children.Add(new StackLayout { Orientation = StackOrientation.Horizontal, Children = { lblFirstName, lblLastName } }, 0, 0);
            grid.Children.Add(lblPhone, 1, 0);

            View = grid;

            //On hold delete
            MenuItem mi = new MenuItem { Text = "Delete", IsDestructive = true };
            mi.Clicked += OnDeleteClicked;
            ContextActions.Add(mi);
        }

        /// <summary>
        /// Helper method for the delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteClicked(object sender, EventArgs e)
        {
            Opponents selectedOpponent = (Opponents)this.BindingContext;

            // Delete all matches associated with the opponent
            App.Database.DeleteOpponent(selectedOpponent);

            // Remove the opponent from the List
            ListView lv = (ListView)this.Parent;
            ((ObservableCollection<Opponents>)lv.ItemsSource).Remove(selectedOpponent);
        }


    }
}