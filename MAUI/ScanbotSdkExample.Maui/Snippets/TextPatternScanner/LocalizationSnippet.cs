using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.TextPattern;

namespace ScanbotSdkExample.Maui.Snippets.TextPatternScanner;

public class LocalizationSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new TextPatternScannerScreenConfiguration();
        
        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.TopUserGuidance = "Localized topUserGuidance";
        localization.CameraPermissionCloseButton = "Localized cameraPermissionCloseButton";
        
        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.Rtu.TextPatternScanner.LaunchAsync(configuration);
       if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }
        
         // Retrieve the value
        // e.g
         Console.WriteLine($"Scanned Text: "+ scannedOutput.Result.RawText); 
    } 
}