using System.Collections.Generic;
using Xamarin.Forms;

namespace DINH3453Assignment2
{
    public class GamesListPage : ContentPage
    {
        public GamesListPage()
        {
            Title = "Games";

            ListView gamesListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(GameCell)),
                RowHeight = 120,
            };

            gamesListView.ItemsSource = GetGamesWithMatchCount();

            Content = gamesListView;
        }

        // Retrieve games with the number of matches for each game
        List<GameWithMatchCount> GetGamesWithMatchCount()
        {
            var games = App.Database.GetGames();
            var gamesWithMatchCount = new List<GameWithMatchCount>();

            foreach (var game in games)
            {
                int matchCount = App.Database.GetMatchCountForGame(game.ID);

                gamesWithMatchCount.Add(new GameWithMatchCount(game, matchCount));
            }

            return gamesWithMatchCount;
        }

        // Helper class to represent a game with its match count
        private class GameWithMatchCount
        {
            public Games Game { get; set; }
            public int MatchCount { get; set; }

            public GameWithMatchCount(Games game, int matchCount)
            {
                Game = game;
                MatchCount = matchCount;
            }
        }
    }

    // ViewCell for displaying each game item in the list
    public class GameCell : ViewCell
    {
        public GameCell()
        {
            var nameLabel = new Label { FontAttributes = FontAttributes.Bold };
            nameLabel.SetBinding(Label.TextProperty, "Game.GameName");

            var descriptionLabel = new Label();
            descriptionLabel.SetBinding(Label.TextProperty, "Game.Description");

            var ratingLabel = new Label();
            ratingLabel.SetBinding(Label.TextProperty, "Game.Rating", stringFormat: "Rating: {0}");

            var matchCountLabel = new Label();
            matchCountLabel.SetBinding(Label.TextProperty, "MatchCount", stringFormat: "Matches: {0}");

            View = new StackLayout
            {
                Padding = new Thickness(10),
                Spacing = 5,
                Children = { nameLabel, descriptionLabel, ratingLabel, matchCountLabel }
            };
        }
    }
}
