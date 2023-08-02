using DocumentSDK.MAUI.Example.Models;
using DocumentSDK.MAUI.Example.Pages;

namespace DocumentSDK.MAUI.Example
{
    public partial class App : Application
    {
        
        public const string LICENSE_KEY = null;
        public App()
        {
            InitializeComponent();
#pragma warning disable CS4014
            // There's no requirement to await this, can just disable warning
            InitializeAsync();
#pragma warning restore CS4014 
            MainPage = new NavigationPage(new HomePage());

        }

        async void InitializeAsync()
        {
            await PageStorage.Instance.InitializeAsync();
            await ScannedPage.Instance.LoadFromStorage();
        }
    }
}

