using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.TextPattern;

namespace ScanbotSdkExample.Maui.Snippets.TextPatternScanner;

public class LaunchSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new TextPatternScannerScreenConfiguration();

        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.Rtu.TextPatternScanner.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Retrieve the value.
        // e.g
        Console.WriteLine($"Scanned Text: " + scannedOutput.Result.RawText);
    }
}