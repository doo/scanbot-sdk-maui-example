using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Maui;
using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Sdk;

namespace ScanbotSdkExample.Maui;

public static partial class MauiProgram
{
    private const string LicenseKey =   "";

    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
        
        // Note: You can enable encryption by setting the 'App.IsEncryptionEnabled' variable to 'true':
        string password = null;
        FileEncryptionMode? encryptionMode = null;
        if (App.IsEncryptionEnabled)
        {
            password = "SomeSecretPa$$w0rdForFileEncryption";
            encryptionMode = FileEncryptionMode.Aes256;
        }

        ScanbotSDKMain.Initialize(builder, new SdkConfiguration
        {
            LicenseKey = LicenseKey,
            LoggingEnabled = true,
            StorageBaseDirectory = StorageBaseDirectoryForExampleApp(),
            StorageImageFormat = StorageImageFormat.Jpg,
            StorageImageQuality = 50,
            // Note: all the images and files exported through the SDK will
            // not be openable from external applications, if they will be
            // encrypted.
           FileEncryptionPassword = password,
           FileEncryptionMode = encryptionMode
        });
        
        return builder.Build();
    }

    private static partial string StorageBaseDirectoryForExampleApp();
}