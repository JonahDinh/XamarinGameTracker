using DINH3453Assignment2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DINH3453Assignment2
{
    public class MatchesPage : ContentPage
    {
        // Saving the ID of the match for updating
        int selectedMatchID = 0;
        // List of matches with Games and opponents (helper class for associated fields)
        ObservableCollection<MatchWithGameAndOpponent> matchesWithGameAndOpponents;
        // Used for persistent storage, saves the last game
        Games lastSelectedGame;

        public MatchesPage(Opponents opponent)
        {
            Title = "Opponents";

            // Error Label (in case all fields aren't filled out)
            Label errorLabel = new Label
            {
                TextColor = Color.Red,
                FontAttributes = FontAttributes.Italic,
                IsVisible = false // Hide initially
            };

            // Date picker with default day
            DatePicker dpDate = new DatePicker { Date = DateTime.Now, WidthRequest = 125 };
            ViewCell vcDate = new ViewCell
            {
                View = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { new Label { Text = "Date:", WidthRequest = 100 }, dpDate }
                }
            };

            EntryCell eComment = new EntryCell { Label = "Comments:", Placeholder = "Enter comments here", HorizontalTextAlignment = TextAlignment.Start };

            // Picker for game, item source is grabbing from the database
            Picker pGame = new Picker
            {
                WidthRequest = 125,
                ItemsSource = App.Database.GetGames(),
            };
            pGame.SelectedIndex = 0;
            //var selectedGameIdString = await SecureStorage.GetAsync("lastSelectedGameID");
            
            pGame.SelectedIndexChanged += async (sender, e) =>
            {
                try
                {
                    // Save the ID of the selected game to SecureStorage
                    Games selectedGame = (Games)pGame.SelectedItem;
                    //await SecureStorage.SetAsync("lastSelectedGameID", selectedGame.ID.ToString());
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                }
            };

            ViewCell vcGame = new ViewCell
            {
                View = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { new Label { Text = "Game:", WidthRequest = 100 }, pGame }
                }
            };

            Switch sWin = new Switch();
            sWin.SetBinding(Switch.IsToggledProperty, "Win");

            ViewCell vcWin = new ViewCell
            {
                View = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { new Label { Text = "Win?", WidthRequest = 100 }, sWin }
                }
            };

            Button btnSave = new Button { Text = "Save" };
            btnSave.Clicked += async (sender, e) =>
            {
                Games selectedGame = (Games)pGame.SelectedItem;
                Matches match = new Matches
                {
                    ID = selectedMatchID,
                    OpponentID = opponent.ID,
                    Date = dpDate.Date,
                    Comments = eComment.Text,
                    GameID = selectedGame.ID,
                    Win = sWin.IsToggled,
                };
                if (dpDate.Date == DateTime.MinValue || string.IsNullOrEmpty(eComment.Text) || pGame.SelectedItem == null)
                {
                    errorLabel.Text = "Please fill in all fields.";
                    errorLabel.IsVisible = true;
                    return;
                }
                errorLabel.IsVisible = false;

                if (selectedMatchID > 0)
                {
                    // Editing an existing match, so update it
                    var matchForUpdate = matchesWithGameAndOpponents.FirstOrDefault(m => m.Match.ID == selectedMatchID);
                    if (matchForUpdate != null)
                    {
                        matchForUpdate.Match = match;
                        // Save the updated match
                        App.Database.SaveMatch(match);
                    }
                }
                else
                {
                    // Adding a new match
                    App.Database.SaveMatch(match);
                    MatchWithGameAndOpponent matchWithGameAndOpponents = new MatchWithGameAndOpponent(selectedGame, opponent, match);
                    matchesWithGameAndOpponents.Add(matchWithGameAndOpponents);
                    await SecureStorage.SetAsync("lastSelectedGame", lastSelectedGame.ToString());
                }

                errorLabel.IsVisible = false;

                // Clear form fields after saving
                ClearForm();

                RefreshListView();
            };

            //Create the associated list
            matchesWithGameAndOpponents = new ObservableCollection<MatchWithGameAndOpponent>(GetMatchesWithGameAndOpponents(opponent.ID));

            ListView listView = new ListView
            {
                ItemsSource = matchesWithGameAndOpponents,
                ItemTemplate = new DataTemplate(typeof(MatchCell)),
                RowHeight = MatchCell.RowHeight,
                HeightRequest = 1450,
                IsPullToRefreshEnabled = true
            };

            listView.Refreshing += async (sender, e) =>
            {
                // Get updated matches
                var updatedMatches = GetMatchesWithGameAndOpponents(opponent.ID);

                // Clear existing matches and add updated matches
                matchesWithGameAndOpponents.Clear();
                foreach (var match in updatedMatches)
                {
                    matchesWithGameAndOpponents.Add(match);
                }

                // End refresh
                listView.EndRefresh();
            };

            listView.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem != null)
                {
                    MatchWithGameAndOpponent selectedMatch = (MatchWithGameAndOpponent)e.SelectedItem;
                    PopulateForm(selectedMatch);
                    listView.SelectedItem = null;
                }
            };

            Content = new StackLayout
            {
                Children =
                {
                    listView,
                    new TableView
                    {
                        Intent = TableIntent.Form,
                        Root = new TableRoot
                        {
                            new TableSection("Add Match")
                            {
                                vcDate, eComment, vcGame, vcWin,

                            },
                        }
                    },
                    btnSave,
                    errorLabel
                }
            };
            // Helper method to retrieve matches with associated game and opponent
            List<MatchWithGameAndOpponent> GetMatchesWithGameAndOpponents(int opponentId)
            {
                var matches = App.Database.GetMatchesForOpponent(opponentId);
                var matchesWithGameAndOpponent = new List<MatchWithGameAndOpponent>();

                foreach (Matches match in matches)
                {
                    Games game = App.Database.GetGameByMatchID(match.ID);
                    matchesWithGameAndOpponent.Add(new MatchWithGameAndOpponent(game, opponent, match));
                }

                return matchesWithGameAndOpponent;
            }
            // Helper method to populate the form fields with selected match details
            void PopulateForm(MatchWithGameAndOpponent selectedMatch)
            {
                selectedMatchID = selectedMatch.Match.ID;
                dpDate.Date = selectedMatch.Match.Date;
                eComment.Text = selectedMatch.Match.Comments;
                //Setting the game to the same as the one on the list
                var game = pGame.ItemsSource.OfType<Games>().FirstOrDefault(g => g.ID == selectedMatch.Game.ID);
                if (game != null)
                {
                    pGame.SelectedItem = game;
                }
                else
                {
                    // Default to Chess if no previously selected game
                    var chessGame = pGame.ItemsSource.OfType<Games>().FirstOrDefault(g => g.GameName == "Chess");
                    if (chessGame != null)
                    {
                        pGame.SelectedItem = chessGame;
                    }
                }
                sWin.IsToggled = selectedMatch.Match.Win;
            }

            // Method for refreshing the listView, clears the list and re-adds from the database
            void RefreshListView()
            {
                var updatedMatches = GetMatchesWithGameAndOpponents(opponent.ID);
                matchesWithGameAndOpponents.Clear();
                foreach (var match in updatedMatches)
                {
                    matchesWithGameAndOpponents.Add(match);
                }
            }

            // Method to clear the form
            void ClearForm()
            {
                selectedMatchID = 0;
                dpDate.Date = DateTime.Now;
                eComment.Text = string.Empty;
                sWin.IsToggled = false;
            }
        }

        // Class representing a match with associated game and opponent
        public class MatchWithGameAndOpponent
        {
            public Games Game { get; set; }
            public Opponents Opponent { get; set; }
            public Matches Match { get; set; }

            public MatchWithGameAndOpponent(Games game, Opponents opponent, Matches match)
            {
                Game = game;
                Opponent = opponent;
                Match = match;
            }
        }

        // Class representing a custom cell for displaying match details in a ListView
        public class MatchCell : ViewCell
        {
            public const int RowHeight = 130; 

            public MatchCell()
            {
                // Create labels for displaying match details
                Label opponentLabel = new Label { FontAttributes = FontAttributes.Bold };
                opponentLabel.SetBinding(Label.TextProperty, new Binding("Opponent.FirstName"));
                Label lastNameLabel = new Label { FontAttributes = FontAttributes.Bold };
                lastNameLabel.SetBinding(Label.TextProperty, new Binding("Opponent.LastName"));

                // Combine first name and last name labels into a stack layout
                StackLayout opponentNameLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { opponentLabel, lastNameLabel }
                };

                Label dateLabel = new Label { FontAttributes = FontAttributes.Bold };
                dateLabel.SetBinding(Label.TextProperty, new Binding("Match.Date", stringFormat: "{0:dddd, MMMM dd, yyyy}"));

                Label commentsLabel = new Label();
                commentsLabel.SetBinding(Label.TextProperty, "Match.Comments");

                Label gameLabel = new Label { FontAttributes = FontAttributes.Bold };
                gameLabel.SetBinding(Label.TextProperty, "Game.GameName");

                Label descriptionLabel = new Label();
                descriptionLabel.SetBinding(Label.TextProperty, "Game.Description");

                Label winQuestionLabel = new Label { Text = "Win?", FontAttributes = FontAttributes.Bold };

                // Create switch for win status
                Switch winSwitch = new Switch
                {
                    IsEnabled = false // Lock the switch
                };
                winSwitch.SetBinding(Switch.IsToggledProperty, "Match.Win");

                // Arrange labels and switch in a grid
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.Children.Add(opponentNameLayout, 0, 0);
                grid.Children.Add(dateLabel, 0, 1);
                grid.Children.Add(commentsLabel, 1, 1);
                grid.Children.Add(gameLabel, 0, 2);
                grid.Children.Add(descriptionLabel, 1, 2);
                grid.Children.Add(winQuestionLabel, 0, 3);
                grid.Children.Add(winSwitch, 1, 3);
                View = grid;

                MenuItem deleteMenuItem = new MenuItem { Text = "Delete", IsDestructive = true };
                deleteMenuItem.Clicked += async (sender, e) =>
                {
                    MatchWithGameAndOpponent matchWithGameAndOpponent = BindingContext as MatchWithGameAndOpponent;
                    App.Database.DeleteMatch(matchWithGameAndOpponent.Match);
                    ListView lv = (ListView)this.Parent;
                    if (lv.ItemsSource is ObservableCollection<MatchWithGameAndOpponent> matches)
                    {
                        matches.Remove(matchWithGameAndOpponent);
                    }
                };

                ContextActions.Add(deleteMenuItem);
            }
        }
    }
}
