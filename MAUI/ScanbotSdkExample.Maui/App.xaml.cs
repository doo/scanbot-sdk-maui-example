namespace ScanbotSdkExample.Maui;

public partial class App
{
    // Enable this variable to turn on the encryption.
    public const bool IsEncryptionEnabled = false;
        
    internal static Page RootPage => Current.MainPage;
    
    internal static INavigation Navigation => RootPage.Navigation;
    
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new HomePage());
    }
}