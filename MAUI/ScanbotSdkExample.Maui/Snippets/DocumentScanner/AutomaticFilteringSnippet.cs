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