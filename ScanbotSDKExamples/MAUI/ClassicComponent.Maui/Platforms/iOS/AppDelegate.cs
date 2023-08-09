using DocumentSDK.MAUI.iOS;
using Foundation;
using DocumentSDK.MAUI.Constants;
using DocumentSDK.MAUI.Models;
using DocumentSDK.MAUI;
using UIKit;

namespace ClassicComponent.Maui;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
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
    internal static UIViewController ExtractViewController(UIWindow window)
    {
        var viewController = window?.RootViewController;

        if (viewController == null) return null;
        
        // If application has a Navigation Controller
        if (viewController is UINavigationController navigationController)
        {
            // ALERT: In case of Navigation Controller we are explicitely accessing the ChildViewController and then returning the last object in stack.
            // Because there is an issue OR we can say it is built this way in MAUI.
            // Condition: The Navigation Controller has a Navigation Renderer in between the hierarchy, so we cannot use "navigationController?.VisibleViewController" 
            return navigationController?.VisibleViewController?.ChildViewControllers?.Last();
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

