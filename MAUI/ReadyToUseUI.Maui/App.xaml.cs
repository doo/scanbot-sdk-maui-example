using ReadyToUseUI.Maui.Models;
using ReadyToUseUI.Maui.Pages;

namespace ReadyToUseUI.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new HomePage());
            
            // SQLite Database migration from ImageFilter to ParametricFilters
            Task.Run(PageStorage.Instance.MigrateTableIfNeeded);
        }
    }
}

