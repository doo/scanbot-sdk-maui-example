using DocumentSDK.MAUI.Constants;
using DocumentSDK.MAUI.iOS;
using DocumentSDK.MAUI.Models;
using Foundation;
using SQLitePCL;

namespace DocumentSDK.MAUI.Example;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => CreateApp();

    private MauiApp CreateApp()
    {
        SBSDKInitializer.Initialize(UIKit.UIApplication.SharedApplication, App.SCANBOT_LICENSE_KEY, new SBSDKConfiguration
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
        raw.SetProvider(new SQLite3Provider_sqlite3());
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

