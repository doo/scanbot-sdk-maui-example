using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Constants;
using ScanbotSDK.MAUI.Models;

namespace ReadyToUseUI.Maui
{
    public static partial class MauiProgram
    {
        internal const string LICENSE_KEY = null;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            SBSDKInitializer.Initialize(LICENSE_KEY, new ScanbotSDK.MAUI.SBSDKConfiguration
            {
                EnableLogging = true,
                StorageBaseDirectory = StorageBaseDirectoryForExampleApp(),
                StorageImageFormat = CameraImageFormat.Jpg,
                StorageImageQuality = 50,
                DetectorType = DocumentDetectorType.MLBased,
                // Uncomment the below to test our encyption functionality.
                //Encryption = new SBSDKEncryption
                //{
                //    Password = "SomeSecretPa$$w0rdForFileEncryption",
                //    Mode = EncryptionMode.AES256
                //}
            });

            return builder.Build();
        }

        private static partial string StorageBaseDirectoryForExampleApp();
    }
}

