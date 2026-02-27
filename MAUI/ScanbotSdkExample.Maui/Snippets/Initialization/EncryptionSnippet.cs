using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Sdk;

namespace ScanbotSdkExample.Maui.Snippets.Initialization;

public class EncryptionSnippet
{
    static void Initialize(MauiAppBuilder builder)
    {
        ScanbotSDKMain.Initialize(builder, new SdkConfiguration
        {
            LicenseKey = "",
            // Note: all the images and files exported through the SDK will
            // not be openable from external applications, if they will be
            // encrypted.
            FileEncryptionPassword = "SomeSecretPa$$w0rdForFileEncryption",
            FileEncryptionMode = FileEncryptionMode.Aes256
        });
    }
}