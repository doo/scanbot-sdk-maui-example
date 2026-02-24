using ScanbotSdkExample.iOS.Controller;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public UINavigationController Controller { get; private set; }

        public override UIWindow Window { get; set; }
        
        /// <summary>
        /// Set the flag to true for enabling encryption.
        /// </summary>
        public const bool IsEncryptionEnabled = false;
        
        /// <summary>
        /// Set the flag to false, for the default storage path.
        /// </summary>
        public const bool SetCustomPath = true;

        private const int ImageQuality = 100;

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
        
        private void InitializeScanbotSdk(UIApplication application)
        {
            // application object
            ScanbotSDKGlobal.SharedApplication = application;

            var nativeConfiguration = ScanbotSDKConfiguration.DefaultConfiguration;

            // Logging
            nativeConfiguration.LoggingEnabled = true;

            if (!string.IsNullOrWhiteSpace(LicenseKey))
            {
                // License
                nativeConfiguration.LicenseString = LicenseKey;
            }

            // Storage Format
            nativeConfiguration.FileStorageImageFormat = SBSDKImageFileFormat.Jpeg;
            nativeConfiguration.FileStorageImageQuality = (byte)ImageQuality;

            // Encryption
            if (IsEncryptionEnabled)
            {
                nativeConfiguration.FileEncryptionMode = SBSDKAESEncrypterMode.SBSDKAESEncrypterModeAES256;
                nativeConfiguration.FileEncryptionPassword = "S0m3W3irDL0ngPa$$w0rdino!!!!";
                
                // Note: all the images and files exported through the SDK will
                // not be openable from external applications, since they will be
                // encrypted.
            }
            
            // Set Storage Url for Native
            var customStoragePath = PageStoragePathForExample();
            var storageUrl = SetCustomPath
                ? customStoragePath
                : SBSDKStorageLocation.DefaultURL;
            nativeConfiguration.FileStorageBaseDirectory = storageUrl;
            
            // Subscribe to License failure handler for logs.
            nativeConfiguration.LicenseFailureHandler = (status, feature, message) =>
            {
                Console.WriteLine($"License status: {status} \nFeature: {feature} \nMessage:{message}");
            };
            
            // Apply Configs
            var success = ScanbotSDKGlobal.ApplyConfiguration(nativeConfiguration);
            if (!success)
            {
                Console.WriteLine("Failed to apply configuration to the ScanbotSDK Initializer.");
            }
        }

        private static NSUrl PageStoragePathForExample()
        {
            // For demo purposes we use a sub-folder in the Documents folder in the Data Container of this App, since the contents can be shared via iTunes.
            // For more detais about the iOS file system see:
            // - https://developer.apple.com/library/archive/documentation/FileManagement/Conceptual/FileSystemProgrammingGuide/FileSystemOverview/FileSystemOverview.html
            // - https://docs.microsoft.com/en-us/xamarin/ios/app-fundamentals/file-system
            // - https://docs.microsoft.com/en-us/dotnet/api/system.environment.specialfolder
            var customDocumentsFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "scanbot-sdk-maui"
            );
            Directory.CreateDirectory(customDocumentsFolder);
            return new NSUrl(customDocumentsFolder, isDir: true);
        }
    }
}
