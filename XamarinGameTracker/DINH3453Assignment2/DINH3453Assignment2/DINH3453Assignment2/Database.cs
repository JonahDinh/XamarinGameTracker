using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DINH3453Assignment2
{
    public class Database
    {
        readonly SQLiteConnection database;
        public Database(string dbPath)
        {
            // Attempt to create the db file or open an existing one
            database = new SQLiteConnection(dbPath);

            // Create tables for Opponents, Matches, and Games if they don't exist
            database.CreateTable<Opponents>();
            database.CreateTable<Matches>();
            database.CreateTable<Games>();

            //Create dummy data
            if (database.Table<Matches>().Count() == 0)
            {
                // Configure new matches
                Matches match1 = new Matches { OpponentID = 1, Date = new DateTime(2024, 3, 15), Comments = "Sample match 1", GameID = 1, Win = true };
                Matches match2 = new Matches { OpponentID = 2, Date = new DateTime(2024, 3, 16), Comments = "Sample match 2", GameID = 2, Win = false };
                Matches match3 = new Matches { OpponentID = 3, Date = new DateTime(2024, 3, 17), Comments = "Sample match 3", GameID = 3, Win = true };
                SaveMatch(match1);
                SaveMatch(match2);
                SaveMatch(match3);
            }

            if (database.Table<Games>().Count() == 0)
            {
                // Configure new games
                Games game1 = new Games { GameName = "Chess", Description = "Simple grid game", Rating = 9.5 };
                Games game2 = new Games { GameName = "Checkers", Description = "Simpler grid game", Rating = 5 };
                Games game3 = new Games { GameName = "Dominoes", Description = "Blocks game", Rating = 6.75 };
                SaveGame(game1);
                SaveGame(game2);
                SaveGame(game3);
            }

            if (database.Table<Opponents>().Count() == 0)
            {
                // Configure new opponents
                Opponents opponent1 = new Opponents { FirstName = "John", LastName = "Doe", Address = "123 Main St", Phone = "555-1234", Email = "john@example.com" };
                Opponents opponent2 = new Opponents { FirstName = "Jane", LastName = "Smith", Address = "456 Elm St", Phone = "555-5678", Email = "jane@example.com" };
                Opponents opponent3 = new Opponents { FirstName = "Alice", LastName = "Johnson", Address = "789 Oak St", Phone = "555-9012", Email = "alice@example.com" };
                SaveOpponent(opponent1);
                SaveOpponent(opponent2);
                SaveOpponent(opponent3);
            }
        }

        // METHODS FOR MATCHES

        public int SaveMatch(Matches match)
        {
            if (match.ID != 0)
            {
                return database.Update(match);
            }
            else
            {
                return database.Insert(match);
            }
        }

        public int DeleteMatch(Matches match)
        {
            return database.Delete(match);
        }
        public List<Matches> GetMatchesForOpponent(int opponentID)
        {
            return database.Table<Matches>().Where(match => match.OpponentID == opponentID).ToList();
        }

        // METHODS FOR OPPOENTS
        public int SaveOpponent(Opponents opponent)
        {
            if (opponent.ID != 0)
            {
                return database.Update(opponent);
            }
            else
            {
                return database.Insert(opponent);
            }
        }

        public int DeleteOpponent(Opponents opponent)
        {
            var matchesToDelete = database.Table<Matches>().Where(m => m.OpponentID == opponent.ID).ToList();

            // Delete each match individually
            foreach (var match in matchesToDelete)
            {
                database.Delete(match);
            }
            return database.Delete(opponent);
        }

        public Games GetGameByMatchID(int matchID)
        {
            var match = database.Table<Matches>().FirstOrDefault(m => m.ID == matchID);
            if (match != null)
            {
                return database.Table<Games>().FirstOrDefault(g => g.ID == match.GameID);
            }
            return null;
        }

        public List<Opponents> GetOpponents()
        {
            return database.Table<Opponents>().ToList();
        }

        // METHODS FOR GAMES

        public int SaveGame(Games game)
        {
            if (game.ID != 0)
            {
                return database.Update(game);
            }
            else
            {
                return database.Insert(game);
            }
        }

        public List<Games> GetGames()
        {
            return database.Table<Games>().ToList();
        }

        public int GetMatchCountForGame(int gameId)
        {
            return database.Table<Matches>().Count(match => match.GameID == gameId);
        }
        public void ResetApp()
        {
            // Drop tables
            database.DropTable<Opponents>();
            database.DropTable<Matches>();
            database.DropTable<Games>();

            // Recreate tables
            database.CreateTable<Opponents>();
            database.CreateTable<Matches>();
            database.CreateTable<Games>();

            if (database.Table<Games>().Count() == 0)
            {
                Games game1 = new Games { GameName = "Chess", Description = "Simple grid game", Rating = 9.5 };
                Games game2 = new Games { GameName = "Checkers", Description = "Simpler grid game", Rating = 5 };
                Games game3 = new Games { GameName = "Dominoes", Description = "Blocks game", Rating = 6.75 };
                SaveGame(game1);
                SaveGame(game2);
                SaveGame(game3);
            }

       
        }

     }
}
