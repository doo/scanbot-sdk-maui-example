using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Vin;

namespace ScanbotSdkExample.Maui.Snippets.VinScanner;

public class FinderOverlaySnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new VinScannerScreenConfiguration();

        // Configure the view finder.
        // Set the style for the view finder.
        // Choose between cornered or stroked style.
        // For default stroked style.
        configuration.ViewFinder.Style = new FinderStrokedStyle();
        // For default cornered style.
        configuration.ViewFinder.Style = new FinderCorneredStyle();
        // You can also set each style's stroke width, stroke color or corner radius.
        // e.g.
        configuration.ViewFinder.Style = new FinderStrokedStyle
        {
            StrokeColor = new ColorValue("#7A000000"),
            CornerRadius = 3.0f,
            StrokeWidth = 2.0f
        };

        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.Vin.StartScannerAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Print the scanned text results
        Console.WriteLine("Scanned Vin Scanner: "+ scannedOutput.Result.TextResult?.RawText);
    } 
}