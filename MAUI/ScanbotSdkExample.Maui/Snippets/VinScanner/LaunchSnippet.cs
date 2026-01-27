using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Vin;

namespace ScanbotSdkExample.Maui.Snippets.VinScanner;

public class LaunchSnippet
{
    public static async Task StartScannerAsync()
    {
        var configuration = new VinScannerScreenConfiguration();

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