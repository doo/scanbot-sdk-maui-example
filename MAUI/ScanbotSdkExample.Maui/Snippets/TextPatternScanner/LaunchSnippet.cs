using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.textpattern;

namespace ScanbotSdkExample.Maui.Snippets.TextPatternScanner;

public class LaunchSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new TextPatternScannerScreenConfiguration();

        // Present the view controller modally.
        var result = await ScanbotSDKMain.Rtu.TextPatternScanner.LaunchAsync(configuration);
       if (result?.Result?.RawText == null)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Retrieve the value.
        // e.g
        Console.WriteLine($"Scanned Text: "+ result.Result.RawText);
    }
}