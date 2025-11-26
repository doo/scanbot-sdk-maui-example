using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Core.Geometry;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class SinglePageSnippet
{
    private static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Set the visibility of the view finder.
        configuration.Screens.Camera.ViewFinder.Visible = true;

        // Create the instance of the style, either `SBSDKUI2FinderCorneredStyle` or `SBSDKUI2FinderStrokedStyle`.
        var style = new FinderCorneredStyle
        {
            StrokeColor = new ColorValue("#FFFFFFFF"),
            StrokeWidth = 3.0,
            CornerRadius = 10.0
        };

        // Set the configured style.
        configuration.Screens.Camera.ViewFinder.Style = style;

        // Set the desired aspect ratio of the view finder.
        configuration.Screens.Camera.ViewFinder.AspectRatio = new AspectRatio(width:4.0,height:5.0);

        // Set the overlay color.
        configuration.Screens.Camera.ViewFinder.OverlayColor = new ColorValue("#26000000");

        // Set the page limit.
        configuration.OutputSettings.PagesScanLimit = 1;

        // Enable the tutorial screen.
        configuration.Screens.Camera.Introduction.ShowAutomatically = true;

        // Disable the acknowledgment screen.
        configuration.Screens.Camera.Acknowledgement.AcknowledgementMode = AcknowledgementMode.None;

        // Disable the review screen.
        configuration.Screens.Review.Enabled = false;

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