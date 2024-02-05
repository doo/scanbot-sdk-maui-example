using Foundation;
using UIKit;

namespace ClassicComponent.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    /// <summary>
    /// Returns the Root Window of the application.
    /// </summary>
    public static UIWindow RootWindow => Current.Window;

    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }
}

