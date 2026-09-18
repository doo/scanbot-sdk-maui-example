using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public class DocumentCleanUpSnippet
{
    private static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Retrieve the instance of the document cleanup configuration from the main configuration object.
        var cleanupScreenConfiguration = configuration.Screens.Cleanup;

        // e.g. enable/disable the buttons. They are by default ON
        cleanupScreenConfiguration.Toolbar.RedoButton.Visible = true;
        cleanupScreenConfiguration.Toolbar.UndoButton.Visible = true;

        // e.g. configure various colors.
        configuration.Appearance.TopBarBackgroundColor = new ColorValue("#C8193C");
        cleanupScreenConfiguration.TopBarConfirmButton.Foreground.Color = Microsoft.Maui.Graphics.Colors.White;

        // e.g. customize a UI element's text
        configuration.Localization.DocumentCleanupTopBarCancelButtonTitle = "Cancel";

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
