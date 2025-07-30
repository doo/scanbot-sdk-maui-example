namespace ScanbotSdkExample.Maui;

public partial class App
{
    internal static Page RootPage => Current.MainPage;
    
    internal static INavigation Navigation => RootPage.Navigation;
    
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new HomePage());
    }
}