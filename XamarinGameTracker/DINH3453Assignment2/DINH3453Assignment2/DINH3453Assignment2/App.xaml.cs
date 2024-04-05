using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DINH3453Assignment2
{
    public partial class App : Application
    {
        static Database database;

        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(DependencyService.Get<IFileHelper>().GetLocalFilePath("DatabaseSQLite.db3"));
                }
                return database;
            }

        }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new OpponentsListPage());

            ToolbarItem gamesToolbarItem = new ToolbarItem { Text = "Games" };
            gamesToolbarItem.Clicked += async (sender, e) =>
            {
                await MainPage.Navigation.PushAsync(new GamesListPage());
            };

            ToolbarItem settingsToolbarItem = new ToolbarItem { Text = "Settings" };
            settingsToolbarItem.Clicked += async (sender, e) =>
            {
                await MainPage.Navigation.PushAsync(new SettingsPage());
            };

            MainPage.ToolbarItems.Add(gamesToolbarItem);
            MainPage.ToolbarItems.Add(settingsToolbarItem);

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }

    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
    }

}
