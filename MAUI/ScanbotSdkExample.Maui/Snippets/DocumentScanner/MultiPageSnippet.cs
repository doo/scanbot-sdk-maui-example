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

        try
        {
            var document = await ScanbotSDKMain.Rtu.DocumentScanner.LaunchAsync(configuration);
            // Handle the document.
        }
        catch (TaskCanceledException)
        {
            // Indicates that the cancel button was tapped.
        }
    }
}