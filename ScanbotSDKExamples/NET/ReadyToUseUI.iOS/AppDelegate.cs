using ReadyToUseUI.iOS.Controller;
using ScanbotSDK.iOS;
using System.Diagnostics;

namespace ReadyToUseUI.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public static float TopInset { get; private set; }

        public UINavigationController Controller { get; private set; }

        public override UIWindow Window { get; set; }

        private const int ImageQuality = 80;

        /// <summary>
        /// Returns the navigation controller object throughout the app.
        /// </summary>
        public static UIViewController NavigationController => (UIApplication.SharedApplication.Delegate as AppDelegate)?.Controller;

        // Please note: The Scanbot SDK will run without a license key for one minute per session!
        // After the trial period is over all Scanbot SDK functions as well as the UI components will stop working.
        // You can get an unrestricted "no-strings-attached" 30 day trial license key for free.
        // Please submit the trial license form (https://scanbot.io/sdk/trial.html) on our website by using
        // the app identifier "io.scanbot.example.sdk.maui.rtu" of this example app.
        private const string LicenseKey = "";

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            InitializeScanbotSdk(application);

            UIViewController initial = new MainViewController();
            Controller = new UINavigationController(initial);

            // Navigation bar background color
            Controller.NavigationBar.BarTintColor = Models.Colors.ScanbotRed;
            // Back button color
            Controller.NavigationBar.TintColor = UIColor.White;
            Controller.NavigationBar.Translucent = false;

            // Title color
            Controller.NavigationBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White,
                Font = UIFont.FromName("HelveticaNeue", 16)
            };

            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            Window.RootViewController = Controller;
            Window.MakeKeyAndVisible();

            return true;
        }

        private static void InitializeScanbotSdk(UIApplication application)
        {
            Debug.WriteLine("Scanbot SDK Example: Initializing Scanbot SDK...");

            ScanbotSDKGlobal.SetLoggingEnabled(true);
            SBSDKUIPageFileStorage.DefaultStorage = new SBSDKUIPageFileStorage(ImageQuality, new SBSDKStorageLocation(NSUrl.FromFilename(PageStoragePathForExample())));
            ScanbotSDKUI.DefaultImageStoreEncrypter = new SBSDKAESEncrypter("S0m3W3irDL0ngPa$$w0rdino!!!!", SBSDKAESEncrypterMode.SBSDKAESEncrypterModeAES128);

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

            var customDocumentsFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "sbsdk-rtu-storage"
            );
            Directory.CreateDirectory(customDocumentsFolder);
            return customDocumentsFolder;
        }
    }
}
