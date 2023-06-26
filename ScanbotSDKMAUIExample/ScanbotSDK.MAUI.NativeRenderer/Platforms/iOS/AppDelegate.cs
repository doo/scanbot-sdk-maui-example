using DocumentSDK.MAUI;
using DocumentSDK.MAUI.Constants;
using DocumentSDK.MAUI.iOS;
using DocumentSDK.MAUI.Models;
using Foundation;
using UIKit;

namespace ScanbotSDK.MAUI.NativeRenderer;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    public static UIViewController CurrentViewController => ExtractViewController();

    protected override MauiApp CreateMauiApp() => CreateApp();

    private MauiApp CreateApp()
    {
        SBSDKInitializer.Initialize(UIKit.UIApplication.SharedApplication, App.LICENSE_KEY, new SBSDKConfiguration
        {
            EnableLogging = true,
            StorageBaseDirectory = GetDemoStorageBaseDirectory(),
            StorageImageFormat = CameraImageFormat.Jpg,
            StorageImageQuality = 50,
            DetectorType = DocumentDetectorType.MLBased,
            Encryption = new SBSDKEncryption
            {
                Password = "SomeSecretPa$$w0rdForFileEncryption",
                Mode = EncryptionMode.AES256
            }
        });

        return MauiProgram.CreateMauiApp();
    }

    string GetDemoStorageBaseDirectory()
    {
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var folder = Path.Combine(documents, "forms-dev-app-storage");
        Directory.CreateDirectory(folder);

        return folder;
    }

    /// <summary>
    /// Show message on top of the Root window
    /// </summary>
    /// <param name="message"></param>
    /// <param name="buttonTitle"></param>
    internal void ShowAlert(string message, string buttonTitle)
    {
        var alert = UIAlertController.Create("Alert", message, UIAlertControllerStyle.Alert);
        var action = UIAlertAction.Create(buttonTitle ?? "Ok", UIAlertActionStyle.Cancel, (obj) => { });
        alert.AddAction(action);
        Window?.RootViewController?.PresentViewController(alert, true, null);
    }

    /// <summary>
    /// Extract ViewController from the Application's ViewController Hierarchy.
    /// </summary>
    /// <returns></returns>
    private static UIViewController ExtractViewController()
    {
        var viewController = UIApplication.SharedApplication.Windows.First().RootViewController;

        // If application has a Navigation Controller
        if (viewController is UINavigationController navigationController)
        {
            return navigationController.VisibleViewController;
        }
        else if (viewController is UITabBarController tabBarController)
        {
            // It is itself a Page renderer.
            return tabBarController.SelectedViewController;
        }
        else
        {   // If application has no Navigation Controller OR TabBarController
            return viewController;
        }
    }
}

