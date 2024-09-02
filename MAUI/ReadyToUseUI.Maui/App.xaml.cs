using ReadyToUseUI.Maui.Pages;

namespace ReadyToUseUI.Maui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new HomePage());
        }
    }
}