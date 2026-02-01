using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.DocumentData;

namespace ScanbotSdkExample.Maui.Snippets.DocumentDataExtractor;

public class FinderOverlaySnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentDataExtractorScreenConfiguration();
        
        // Configure the view finder.
        // Set the style for the view finder.
        // Choose between cornered or stroked style.
        // For default stroked style.
        configuration.ViewFinder.Style = new FinderStrokedStyle();
        // For default cornered style.
        configuration.ViewFinder.Style = new FinderCorneredStyle();
        // You can also set each style's stroke width, stroke color or corner radius.
        // e.g
        configuration.ViewFinder.Style = new FinderStrokedStyle
        {
            StrokeColor = new ColorValue("#7A000000"),
            CornerRadius = 3.0f,
            StrokeWidth = 2.0f
        };

        // Present the view controller modally.
        var result = await ScanbotSDKMain.DocumentDataExtractor.StartExtractorScreenAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }
        
        // Iterate through all the document fields
        foreach (var field in result.Value.Document.Fields)
        {
            Console.WriteLine($"{field.Type.Name}: {field.Value.Text}");
        }
    } 
}