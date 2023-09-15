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
    /// <summary>
    /// Returns the Root Window of the application.
    /// </summary>
    public static UIWindow RootWindow => (UIApplication.SharedApplication.Delegate as AppDelegate).Window;

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
}

