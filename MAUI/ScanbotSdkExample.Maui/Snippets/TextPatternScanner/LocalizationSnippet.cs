using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.TextPattern;

namespace ScanbotSdkExample.Maui.Snippets.TextPatternScanner;

public class LocalizationSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new TextPatternScannerScreenConfiguration();

        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.TopUserGuidance = "Localized topUserGuidance";
        localization.CameraPermissionCloseButton = "Localized cameraPermissionCloseButton";

        // Present the view controller modally.
        var result = await ScanbotSDKMain.TextPattern.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }

        // Retrieve the value
        // e.g
        Console.WriteLine($"Scanned Text: " + result.Value.RawText);
    }
}