using ScanbotSdkExample.Maui.Pages;

namespace ScanbotSdkExample.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new HomePage());
    }
}