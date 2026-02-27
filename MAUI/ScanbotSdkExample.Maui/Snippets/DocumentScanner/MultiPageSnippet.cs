using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class MultiPageSnippet
{
    private static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Set the page limit to 0, to disable the limit, or set it to the number of pages you want to scan.
        configuration.OutputSettings.PagesScanLimit = 0;

        // Disable the acknowledgment screen.
        configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = AcknowledgementMode.None;

        // Launch the scanner
        var result = await ScanbotSDKMain.Document.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }
        
        // Handle the document.
        var scannedDocument = result.Value;
    }
}