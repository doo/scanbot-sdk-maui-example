using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ReadyToUseUI.Maui;

public partial class Snippets
{
    private static async Task AutomaticFilteringSnippet()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();
        
        // setting default parametric filter
        configuration.OutputSettings.DefaultFilter = ParametricFilter.ColorDocument;

        // e.g. configure various colors.
        configuration.Appearance.TopBarBackgroundColor = new ColorValue("#C8193C");

        // e.g. customize a UI element's text
        configuration.Localization.CroppingTopBarCancelButtonTitle = "Cancel";

        try
        {
            var document = await ScanbotSDKMain.RTU.DocumentScanner.LaunchAsync(configuration);
            // Handle the document.
        }
        catch (TaskCanceledException)
        {
            // Indicates that the cancel button was tapped.
        }
    }
}