namespace ScanbotSdkExample.Maui;

public partial class App
{
    /// <summary>
    /// Enable this variable to turn on the encryption.
    /// </summary>
    public const bool IsEncryptionEnabled = false;

    /// <summary>
    /// Constant timespan value of 3 seconds to force close the scanner.
    /// </summary>
    public const int ForceCloseInterval = 3000;
    
    /// <summary>
    /// Turns on the test code for Force closing the RTU UI scanners.
    /// </summary>
    internal const bool TestForceCloseFeature = true;
    
    /// <summary>
    /// Returns the NavigationPage object by accessing the singleton CurrentWindow instance.
    /// </summary>
    public static NavigationPage Navigation => CurrentWindow?.Page as NavigationPage;

    private static Window _currentWindow;
    /// <summary>
    /// Singleton Window object to retrieve the Navigation of the App.
    /// </summary>
    public static Window CurrentWindow
    {
        get
        {
            if (_currentWindow == null)
            {
                _currentWindow = new Window(new NavigationPage(new HomePage()));
            }

            return _currentWindow;
        }
    }

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState activationState) => CurrentWindow;
}