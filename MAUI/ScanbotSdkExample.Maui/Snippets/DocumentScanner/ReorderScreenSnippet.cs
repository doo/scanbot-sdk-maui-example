using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class ReorderScreenSnippet
{
    private static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Retrieve the instance of the reorder pages configuration from the main configuration object.
        var reorderScreenConfiguration = configuration.Screens.ReorderPages;

        // Hide the guidance view.
        reorderScreenConfiguration.Guidance.Visible = false;

        // Set the title for the reorder screen.
        reorderScreenConfiguration.TopBarTitle.Text = "Reorder Pages Screen";

        // Set the title for the guidance.
        reorderScreenConfiguration.Guidance.Title.Text = "Reorder";

        // Set the color for the page number text.
        reorderScreenConfiguration.PageTextStyle.Color = Microsoft.Maui.Graphics.Colors.Black;

        // Apply the configurations.
        configuration.Screens.ReorderPages = reorderScreenConfiguration;
        
        // Launch the scanner
        var response = await ScanbotSDKMain.Document.StartScannerAsync(configuration);
        if (response.Status != OperationResult.Ok)
        {
            // Indicates that the cancel button was tapped.
            return;
        }
        
        // Handle the document.
        var scannedDocument = response.Result;
    }
}