using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Vin;

namespace ScanbotSdkExample.Maui.Snippets.VinScanner;

public class LocalizationSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new VinScannerScreenConfiguration();
        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.TopUserGuidance = "Localized topUserGuidance";
        localization.CameraPermissionCloseButton = "Localized cameraPermissionCloseButton";
        
        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.Rtu.VinScanner.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Print the scanned text results
        Console.WriteLine("Scanned Vin Scanner: "+ scannedOutput.Result.TextResult?.RawText);
    } 
}