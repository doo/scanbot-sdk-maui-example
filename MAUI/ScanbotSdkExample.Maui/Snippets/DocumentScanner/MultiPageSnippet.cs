using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class MultiPageSnippet
{
    private static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Set the page limit to 0, to disable the limit, or set it to the number of pages you want to scan.
        configuration.OutputSettings.PagesScanLimit = 0;

        // Disable the acknowledgment screen.
        configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = AcknowledgementMode.None;

        // Launch the scanner
        var response = await ScanbotSDKMain.Rtu.DocumentScanner.LaunchAsync(configuration);
        if (response.Status != OperationResult.Ok)
        {
            // Indicates that the cancel button was tapped.
            return;
        }
        
        // Handle the document.
        var scannerDocument = response.Result;
    }
}