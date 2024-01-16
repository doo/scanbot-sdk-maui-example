using System.Diagnostics;
using ScanbotSDK.iOS;

namespace ClassicComponent.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // Use a custom temp storage directory for demo purposes.
        public static string Directory { get; private set; }

        // TODO Add the Scanbot SDK license key here.
        // Please note: The Scanbot SDK will run without a license key for one minute per session!
        // After the trial period is over all Scanbot SDK functions as well as the UI components will stop working
        // or may be terminated. You can get an unrestricted "no-strings-attached" 30 day trial license key for free.
        // Please submit the trial license form (https://scanbot.io/sdk/trial.html) on our website by using
        // the app identifier "io.scanbot.example.sdk.net" of this example app.
        const string LicenseKey = null;

        public override UIWindow Window { get; set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            InitializeScanbotSdk(application);
            return true;
        }

        private static void InitializeScanbotSdk(UIApplication application)
        {
            Debug.WriteLine("Scanbot SDK Example: Initializing Scanbot SDK...");
            Directory = PageStoragePathForExample();

            ScanbotSDKGlobal.SetLoggingEnabled(true);
            ScanbotSDKUI.DefaultImageStoreEncrypter = new SBSDKAESEncrypter("S0m3W3irDL0ngPa$$w0rdino!!!!", SBSDKAESEncrypterMode.SBSDKAESEncrypterModeAES256);
            if (!string.IsNullOrEmpty(LicenseKey))
            {
                ScanbotSDKGlobal.SetLicense(LicenseKey);
            }

            ScanbotSDKGlobal.SetupDefaultLicenseFailureHandler();
            ScanbotSDKGlobal.SharedApplication = application;
        }

        private static string PageStoragePathForExample()
        {
            // For demo purposes we use a sub-folder in the Documents folder in the Data Container of this App, since the contents can be shared via iTunes.
            // For more detais about the iOS file system see:
            // - https://developer.apple.com/library/archive/documentation/FileManagement/Conceptual/FileSystemProgrammingGuide/FileSystemOverview/FileSystemOverview.html
            // - https://docs.microsoft.com/en-us/xamarin/ios/app-fundamentals/file-system
            // - https://docs.microsoft.com/en-us/dotnet/api/system.environment.specialfolder

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var exampleTempStorage = System.IO.Path.Combine(documents, "sbsdk-maui-cc-storage");
            System.IO.Directory.CreateDirectory(exampleTempStorage);
            return exampleTempStorage;
        }
    }
}