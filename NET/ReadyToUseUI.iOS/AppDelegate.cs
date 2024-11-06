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
        private const string LicenseKey =   "KGO0GonuTDTEkO8dfW8mFww63sBu6q" +
  "/nHu5SkZsIe+dG4Uy68FFFUgi72PCz" +
  "W24kq7BR5ENcyPhrSedhiJYtibUEfh" +
  "9jKMB82xyIyIsz5WT6fpwFRWg1Y/OT" +
  "SnOf2eznVycM8t3c5P+Kw/MTERRWvh" +
  "zFnZ4AwkmIfDUGkxfG1D219Lfu4iCs" +
  "ZHyX5CvldzKJActqd2jMFoq5exW2ST" +
  "6VkCo8ssAEJqJ9eO/F9zJJipxqWtJg" +
  "MwXcLAD1HHfQldRN4uW97WgawQDGd+" +
  "MEFHSa2Ylt2kzqtle1GzZ66jyHOZow" +
  "y2bCuu0X7pDLJ2eUKQNFEfgopXhwFI" +
  "U04SCWSJ03Og==\nU2NhbmJvdFNESw" +
  "pkb28uc2NhbmJvdC5jYXBhY2l0b3Iu" +
  "ZXhhbXBsZXxpby5zY2FuYm90LmV4YW" +
  "1wbGUuZG9jdW1lbnQudXNlY2FzZXMu" +
  "YW5kcm9pZHxpby5zY2FuYm90LmV4YW" +
  "1wbGUuZG9jdW1lbnRzZGsudXNlY2Fz" +
  "ZXMuaW9zfGlvLnNjYW5ib3QuZXhhbX" +
  "BsZS5mbHV0dGVyfGlvLnNjYW5ib3Qu" +
  "ZXhhbXBsZS5zZGsuYW5kcm9pZHxpby" +
  "5zY2FuYm90LmV4YW1wbGUuc2RrLmJh" +
  "cmNvZGUuYW5kcm9pZHxpby5zY2FuYm" +
  "90LmV4YW1wbGUuc2RrLmJhcmNvZGUu" +
  "Y2FwYWNpdG9yfGlvLnNjYW5ib3QuZX" +
  "hhbXBsZS5zZGsuYmFyY29kZS5mbHV0" +
  "dGVyfGlvLnNjYW5ib3QuZXhhbXBsZS" +
  "5zZGsuYmFyY29kZS5pb25pY3xpby5z" +
  "Y2FuYm90LmV4YW1wbGUuc2RrLmJhcm" +
  "NvZGUubWF1aXxpby5zY2FuYm90LmV4" +
  "YW1wbGUuc2RrLmJhcmNvZGUubmV0fG" +
  "lvLnNjYW5ib3QuZXhhbXBsZS5zZGsu" +
  "YmFyY29kZS5yZWFjdG5hdGl2ZXxpby" +
  "5zY2FuYm90LmV4YW1wbGUuc2RrLmJh" +
  "cmNvZGUud2luZG93c3xpby5zY2FuYm" +
  "90LmV4YW1wbGUuc2RrLmJhcmNvZGUu" +
  "eGFtYXJpbnxpby5zY2FuYm90LmV4YW" +
  "1wbGUuc2RrLmJhcmNvZGUueGFtYXJp" +
  "bi5mb3Jtc3xpby5zY2FuYm90LmV4YW" +
  "1wbGUuc2RrLmNhcGFjaXRvcnxpby5z" +
  "Y2FuYm90LmV4YW1wbGUuc2RrLmNhcG" +
  "FjaXRvci5hbmd1bGFyfGlvLnNjYW5i" +
  "b3QuZXhhbXBsZS5zZGsuY2FwYWNpdG" +
  "9yLmlvbmljfGlvLnNjYW5ib3QuZXhh" +
  "bXBsZS5zZGsuY2FwYWNpdG9yLmlvbm" +
  "ljLnJlYWN0fGlvLnNjYW5ib3QuZXhh" +
  "bXBsZS5zZGsuY2FwYWNpdG9yLmlvbm" +
  "ljLnZ1ZWpzfGlvLnNjYW5ib3QuZXhh" +
  "bXBsZS5zZGsuY29yZG92YS5pb25pY3" +
  "xpby5zY2FuYm90LmV4YW1wbGUuc2Rr" +
  "LmZsdXR0ZXJ8aW8uc2NhbmJvdC5leG" +
  "FtcGxlLnNkay5pb3MuYmFyY29kZXxp" +
  "by5zY2FuYm90LmV4YW1wbGUuc2RrLm" +
  "lvcy5jbGFzc2ljfGlvLnNjYW5ib3Qu" +
  "ZXhhbXBsZS5zZGsuaW9zLnJ0dXVpfG" +
  "lvLnNjYW5ib3QuZXhhbXBsZS5zZGsu" +
  "bWF1aXxpby5zY2FuYm90LmV4YW1wbG" +
  "Uuc2RrLm1hdWkucnR1fGlvLnNjYW5i" +
  "b3QuZXhhbXBsZS5zZGsubmV0fGlvLn" +
  "NjYW5ib3QuZXhhbXBsZS5zZGsucmVh" +
  "Y3RuYXRpdmV8aW8uc2NhbmJvdC5leG" +
  "FtcGxlLnNkay5yZWFjdC5uYXRpdmV8" +
  "aW8uc2NhbmJvdC5leGFtcGxlLnNkay" +
  "5ydHUuYW5kcm9pZHxpby5zY2FuYm90" +
  "LmV4YW1wbGUuc2RrLnhhbWFyaW58aW" +
  "8uc2NhbmJvdC5leGFtcGxlLnNkay54" +
  "YW1hcmluLmZvcm1zfGlvLnNjYW5ib3" +
  "QuZXhhbXBsZS5zZGsueGFtYXJpbi5y" +
  "dHV8aW8uc2NhbmJvdC5mb3Jtcy5uYX" +
  "RpdmVyZW5kZXJlcnMuZXhhbXBsZXxp" +
  "by5zY2FuYm90Lm5hdGl2ZWJhcmNvZG" +
  "VzZGtyZW5kZXJlcnxpby5zY2FuYm90" +
  "LlNjYW5ib3RTREtTd2lmdFVJRGVtb3" +
  "xpby5zY2FuYm90LnNka193cmFwcGVy" +
  "LmRlbW8uYmFyY29kZXxpby5zY2FuYm" +
  "90LnNkay13cmFwcGVyLmRlbW8uYmFy" +
  "Y29kZXxpby5zY2FuYm90LnNka193cm" +
  "FwcGVyLmRlbW8uZG9jdW1lbnR8aW8u" +
  "c2NhbmJvdC5zZGstd3JhcHBlci5kZW" +
  "1vLmRvY3VtZW50fGlvLnNjYW5ib3Qu" +
  "c2RrLmludGVybmFsZGVtb3xsb2NhbG" +
  "hvc3R8T3BlcmF0aW5nU3lzdGVtU3Rh" +
  "bmRhbG9uZXxzY2FuYm90c2RrLXFhLT" +
  "EuczMtZXUtd2VzdC0xLmFtYXpvbmF3" +
  "cy5jb218c2NhbmJvdHNkay1xYS0yLn" +
  "MzLWV1LXdlc3QtMS5hbWF6b25hd3Mu" +
  "Y29tfHNjYW5ib3RzZGstcWEtMy5zMy" +
  "1ldS13ZXN0LTEuYW1hem9uYXdzLmNv" +
  "bXxzY2FuYm90c2RrLXFhLTQuczMtZX" +
  "Utd2VzdC0xLmFtYXpvbmF3cy5jb218" +
  "c2NhbmJvdHNkay1xYS01LnMzLWV1LX" +
  "dlc3QtMS5hbWF6b25hd3MuY29tfHNj" +
  "YW5ib3RzZGstd2FzbS1kZWJ1Z2hvc3" +
  "QuczMtZXUtd2VzdC0xLmFtYXpvbmF3" +
  "cy5jb218d2Vic2RrLWRlbW8taW50ZX" +
  "JuYWwuc2NhbmJvdC5pb3wqLnFhLnNj" +
  "YW5ib3QuaW8KMTczMzA5NzU5OQo4Mz" +
  "g4NjA3CjMx\n";

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
            SBSDKDocumentPageFileStorage.DefaultStorage = new SBSDKDocumentPageFileStorage(SBSDKImageFileFormat.Jpeg, (nuint)ImageQuality, new SBSDKStorageLocation(NSUrl.FromFilename(PageStoragePathForExample())));

            // Uncomment the below to test our encyption functionality.
            // ScanbotUI.DefaultImageStoreEncrypter = new SBSDKAESEncrypter("S0m3W3irDL0ngPa$$w0rdino!!!!", SBSDKAESEncrypterMode.SBSDKAESEncrypterModeAES128);
            // Note: all the images and files exported through the SDK will
            // not be openable from external applications, since they will be
            // encrypted.

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
