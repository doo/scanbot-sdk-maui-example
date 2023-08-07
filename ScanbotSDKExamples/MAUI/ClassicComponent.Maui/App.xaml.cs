using BarcodeSDK.MAUI.Models;
using Xamarin.Google.Crypto.Tink.Proto;

namespace ClassicComponent.Maui;
/// <summary>
/// Type of application.
/// </summary>
enum ApplicationType
{
    SinglePage,
    NavigationPage,
    TabbedPage
}

public partial class App : Application
{
    public const string LICENSE_KEY = null;

    // JUST FOR DEBUG PURPOSES.
    // Using this constant we can switch between different application
    // structures (SinglePage, NavigationPage, TabbedPage) to make
    // sure everything works under every circumstance.
    const ApplicationType APPLICATION_TYPE = ApplicationType.SinglePage;

    public App()
    {
        InitializeComponent();
        MainPage = GetMainPage(applicationType: APPLICATION_TYPE);
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

    /// <summary>
    /// Get the Application Main Page by selecting the structure mentioned.
    /// </summary>
    /// <param name="applicationType">Specify structure of the application</param>
    /// <returns>Returns the page object</returns>
    private Page GetMainPage(ApplicationType applicationType)
    {
        switch (applicationType)
        {
            case ApplicationType.SinglePage:
                return new MainPage();
            case ApplicationType.NavigationPage:
                return new NavigationPage(new MainPage());
            case ApplicationType.TabbedPage:
                var tabbedPage = new TabbedPage();
                tabbedPage.Children.Add(new MainPage { Title = "1" });
                tabbedPage.Children.Add(new SecondPage { Title = "2" });
                tabbedPage.Children.Add(new ThirdPage { Title = "3" });
                return tabbedPage;
            default:
                return new MainPage();

        }
    }

    public class SecondPage : ContentPage { }

    public class ThirdPage : ContentPage { }
}

