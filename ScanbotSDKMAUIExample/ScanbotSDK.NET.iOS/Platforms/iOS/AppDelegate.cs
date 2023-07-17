using System.Runtime.InteropServices;
using DocumentSDK.MAUI.Example.Native.iOS.Controller;
using SBSDK = DocumentSDK.MAUI.Native.iOS.ScanbotSDK;
using Foundation;
using UIKit;
using DocumentSDK.MAUI.Constants;

namespace DocumentSDK.MAUI.Example.Native.iOS
{

    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // TODO Add the Scanbot SDK license key here.
        // Please note: The Scanbot SDK will run without a license key for one minute per session!
        // After the trial period is over all Scanbot SDK functions as well as the UI components will stop working.
        // You can get an unrestricted "no-strings-attached" 30 day trial license key for free.
        // Please submit the trial license form (https://scanbot.io/sdk/trial.html) on our website by using
        // the app identifier "io.scanbot.example.sdk.maui.rtu" of this example app.
        const string LICENSE_KEY = null;

        public static float TopInset { get; private set; }

        public UINavigationController Controller { get; set; }

        public override UIWindow Window { get; set; }

        /// <summary>
        /// Returns the navigation controller object throughout the app.
        /// </summary>
        public static UIViewController NavigationController => (UIApplication.SharedApplication.Delegate as AppDelegate)?.Controller;

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Console.WriteLine("Scanbot SDK Example: Initializing Scanbot SDK...");

            // Initialization with a custom, public(!) "StorageBaseDirectory" for demo purposes - see comments below!
            var configuration = new DocumentSDK.MAUI.Native.iOS.SBSDKConfiguration
            {
                EnableLogging = true,
                StorageBaseDirectory = GetDemoStorageBaseDirectory(),
                Encryption = new MAUI.Models.SBSDKEncryption
                {
                    Mode = EncryptionMode.AES256,
                    Password = "S0m3W3irDL0ngPa$$w0rdino!!!!"
                }
            };
            SBSDK.Initialize(application, LICENSE_KEY, configuration);

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

            //TopInset = (float)(Controller.NavigationBar.Frame.Height + UIApplication.SharedApplication.StatusBarFrame.Height);

            Window.MakeKeyAndVisible();

            return true;
        }

        string GetDemoStorageBaseDirectory()
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
