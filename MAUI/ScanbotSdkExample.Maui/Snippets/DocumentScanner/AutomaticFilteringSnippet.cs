using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public class AutomaticFilteringSnippet
{
    private static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // setting default parametric filter
        configuration.OutputSettings.DefaultFilter = ParametricFilter.ColorDocument;

        // e.g. configure various colors.
        configuration.Appearance.TopBarBackgroundColor = new ColorValue("#C8193C");

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