using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Core.DocumentScanner;

namespace ScanbotSdkExample.Maui;

public static partial class MauiProgram
{
    private const string LicenseKey = "";

    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        // Note: You can enable encryption by setting the 'App.IsEncryptionEnabled' variable to 'true':
        SBSDKEncryption encryption = null;
        if (App.IsEncryptionEnabled)
        {
            encryption = new SBSDKEncryption
            {
                Password = "SomeSecretPa$$w0rdForFileEncryption",
                Mode = StorageEncryptionMode.Aes256
            };
        }

        ScanbotSdkMain.Initialize(builder, LicenseKey, new ScanbotSdkConfiguration
        {
            EnableLogging = true,
            StorageBaseDirectory = StorageBaseDirectoryForExampleApp(),
            StorageImageFormat = CameraImageFormat.Jpg,
            StorageImageQuality = 50,
            EngineMode = DocumentScannerEngineMode.Ml,
            // Note: all the images and files exported through the SDK will
            // not be openable from external applications, if they will be
            // encrypted.
            Encryption = encryption
        });
        
        return builder.Build();
    }

    private static partial string StorageBaseDirectoryForExampleApp();
}