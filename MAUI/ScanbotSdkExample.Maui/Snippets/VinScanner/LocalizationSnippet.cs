using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Vin;

namespace ScanbotSdkExample.Maui.Snippets.VinScanner;

public class LocalizationSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new VinScannerScreenConfiguration();
        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.TopUserGuidance = "Localized topUserGuidance";
        localization.CameraPermissionCloseButton = "Localized cameraPermissionCloseButton";
        
        // Present the view controller modally.
        var result = await ScanbotSDKMain.Vin.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }

        // Print the scanned text results
        Console.WriteLine("Scanned Vin Scanner: "+ result.Value.TextResult?.RawText);
    } 
}