﻿using ScanbotSDK.MAUI;

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
                // You can enable encryption by uncommenting the following lines:
                //Encryption = new SBSDKEncryption
                //{
                //    Password = "SomeSecretPa$$w0rdForFileEncryption",
                //    Mode = EncryptionMode.AES256
                //}
                // Note: all the images and files exported through the SDK will
                // not be openable from external applications, since they will be
                // encrypted.
            });

            return builder.Build();
        }

        private static partial string StorageBaseDirectoryForExampleApp();
    }
}

