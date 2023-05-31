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
        // the app identifier "io.scanbot.example.sdk.xamarin.rtu" of this example app.
        const string LICENSE_KEY = "VKBVNv4brhBHL+Gz3ZJ8K2TeUs+197" +
  "NR0MoDNuMK6cbpoAd9zZMzQrAotxi5" +
  "JZ3jo+UotV5mhuz4KY0MRI9T/VmLcD" +
  "5TEnP+X0+ak6wti5Jge85xqSQUIKZP" +
  "Vz8XGbpGKCMFc4gLf9csO3FpMrptX/" +
  "wVcSUj6kHXrnNVvgu6UYmZ+UZ3l8aT" +
  "SE+tTkV8b9yGTK+zX+eyHm2LL4FveY" +
  "HBPw2reXZOjltPe3AHxDpMj+kG2PrR" +
  "5Dwl27nhrrKSc3H3SE2vbghSf75DMC" +
  "ah2n0C9QOoo4RoB/CEFxTSy4uWp051" +
  "4cYohIj8ksdyP3YW98WwQTv0pDFJWk" +
  "83ovH7gcezEw==\nU2NhbmJvdFNESw" +
  "ppby5zY2FuYm90LmV4YW1wbGUuZmx1" +
  "dHRlcnxpby5zY2FuYm90LmV4YW1wbG" +
  "Uuc2RrLmFuZHJvaWR8aW8uc2NhbmJv" +
  "dC5leGFtcGxlLnNkay5iYXJjb2RlLm" +
  "FuZHJvaWR8aW8uc2NhbmJvdC5leGFt" +
  "cGxlLnNkay5iYXJjb2RlLmZsdXR0ZX" +
  "J8aW8uc2NhbmJvdC5leGFtcGxlLnNk" +
  "ay5iYXJjb2RlLmlvbmljfGlvLnNjYW" +
  "5ib3QuZXhhbXBsZS5zZGsuYmFyY29k" +
  "ZS5yZWFjdG5hdGl2ZXxpby5zY2FuYm" +
  "90LmV4YW1wbGUuc2RrLmJhcmNvZGUu" +
  "d2luZG93c3xpby5zY2FuYm90LmV4YW" +
  "1wbGUuc2RrLmJhcmNvZGUueGFtYXJp" +
  "bnxpby5zY2FuYm90LmV4YW1wbGUuc2" +
  "RrLmJhcmNvZGUueGFtYXJpbi5mb3Jt" +
  "c3xpby5zY2FuYm90LmV4YW1wbGUuc2" +
  "RrLmNhcGFjaXRvci5pb25pY3xpby5z" +
  "Y2FuYm90LmV4YW1wbGUuc2RrLmNvcm" +
  "RvdmEuaW9uaWN8aW8uc2NhbmJvdC5l" +
  "eGFtcGxlLnNkay5mbHV0dGVyfGlvLn" +
  "NjYW5ib3QuZXhhbXBsZS5zZGsuaW9z" +
  "LmJhcmNvZGV8aW8uc2NhbmJvdC5leG" +
  "FtcGxlLnNkay5pb3MuY2xhc3NpY3xp" +
  "by5zY2FuYm90LmV4YW1wbGUuc2RrLm" +
  "lvcy5ydHV1aXxpby5zY2FuYm90LmV4" +
  "YW1wbGUuc2RrLnJlYWN0bmF0aXZlfG" +
  "lvLnNjYW5ib3QuZXhhbXBsZS5zZGsu" +
  "cmVhY3QubmF0aXZlfGlvLnNjYW5ib3" +
  "QuZXhhbXBsZS5zZGsucnR1LmFuZHJv" +
  "aWR8aW8uc2NhbmJvdC5leGFtcGxlLn" +
  "Nkay54YW1hcmlufGlvLnNjYW5ib3Qu" +
  "ZXhhbXBsZS5zZGsueGFtYXJpbi5mb3" +
  "Jtc3xpby5zY2FuYm90LmV4YW1wbGUu" +
  "c2RrLnhhbWFyaW4ucnR1fGlvLnNjYW" +
  "5ib3QubmF0aXZlYmFyY29kZXNka3Jl" +
  "bmRlcmVyfGlvLnNjYW5ib3Quc2RrLm" +
  "ludGVybmFsZGVtb3xsb2NhbGhvc3R8" +
  "c2NhbmJvdHNkay13YXNtLWRlYnVnaG" +
  "9zdC5zMy1ldS13ZXN0LTEuYW1hem9u" +
  "YXdzLmNvbXx3ZWJzZGstZGVtby1pbn" +
  "Rlcm5hbC5zY2FuYm90LmlvCjE2ODU2" +
  "NjM5OTkKODM4ODYwNwoyNw==\n";

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
