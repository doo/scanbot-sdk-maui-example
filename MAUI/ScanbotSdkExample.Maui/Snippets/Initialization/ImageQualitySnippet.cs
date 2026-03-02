using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Sdk;

namespace ScanbotSdkExample.Maui.Snippets.Initialization;

public class ImageQualitySnippet
{
    static void Initialize(MauiAppBuilder builder)
    {
        ScanbotSDKMain.Initialize(builder, new SdkConfiguration
        {
            LicenseKey = "",
            StorageImageFormat = StorageImageFormat.Jpg,
            StorageImageQuality = 50,
        });
    }
}