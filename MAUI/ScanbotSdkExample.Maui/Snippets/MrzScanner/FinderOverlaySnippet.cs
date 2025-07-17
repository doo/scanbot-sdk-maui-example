using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSDK.MAUI.MRZ;

namespace ScanbotSdkExample.Maui.Snippets.MrzScanner;

public class FinderOverlaySnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new MrzScannerScreenConfiguration();

        // To hide the example layout preset.
        configuration.MrzExampleOverlay = new NoLayoutPreset();

        // Configure the finder example overlay. You can choose between the two-line and three-line preset.
        // Each example preset has a default text for each line, but you can change it according to your liking.
        // Each preset has a fixed aspect ratio adjusted to its number of lines. To override, please use 'aspectRatio'
        // parameter in 'viewFinder' field in the main configuration object.

        // To use the default ones.
        configuration.MrzExampleOverlay = new TwoLineMrzFinderLayoutPreset();
        configuration.MrzExampleOverlay = new ThreeLineMrzFinderLayoutPreset();

        // Or configure the preset.
        // For this example we will configure the three-line preset.
        var mrzFinderLayoutPreset = new ThreeLineMrzFinderLayoutPreset
        {
            MrzTextLine1 = "I<USA2342353464<<<<<<<<<<<<<<<",
            MrzTextLine2 = "9602300M2904076USA<<<<<<<<<<<2",
            MrzTextLine3 = "SMITH<<JACK<<<<<<<<<<<<<<<<<<<"
        };

        // Set the configured finder layout preset on the main configuration object.
        configuration.MrzExampleOverlay = mrzFinderLayoutPreset;

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
        var result = await ScanbotSDKMain.Rtu.MrzScanner.LaunchAsync(configuration);
        if (result.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed mrz class.
        var mrz = new MRZ(result.Result.MrzDocument);

        // Retrieve the values.
        // e.g
        Console.WriteLine($"Birth Date: {mrz.BirthDate.Value.Text}, Nationality: {mrz.Nationality.Value.Text}");
    }
}