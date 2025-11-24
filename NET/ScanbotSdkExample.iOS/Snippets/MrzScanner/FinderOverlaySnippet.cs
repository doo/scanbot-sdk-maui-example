using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.MRZScanner;

class FinderOverlaySnippet : UIViewController
{
    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        // Start scanning here. Usually this is an action triggered by some button or menu.
        StartScanning();
    }

    void StartScanning()
    {
        // Create the default configuration object.
        var configuration = new SBSDKUI2MRZScannerScreenConfiguration();

        // To hide the example layout preset.
        configuration.MrzExampleOverlay = SBSDKUI2MRZFinderLayoutPreset.NoLayoutPreset;

        // Configure the finder example overlay. You can choose between the two-line and three-line preset.
        // Each example preset has a default text for each line, but you can change it according to your liking.
        // Each preset has a fixed aspect ratio adjusted to its number of lines. To override, please use 'aspectRatio'
        // parameter in 'viewFinder' field in the main configuration object.

        // To use the default ones.
        configuration.MrzExampleOverlay = SBSDKUI2MRZFinderLayoutPreset.TwoLineMrzFinderLayoutPreset;
        configuration.MrzExampleOverlay = SBSDKUI2MRZFinderLayoutPreset.ThreeLineMrzFinderLayoutPreset;

        // Or configure the preset.
        // For this example we will configure the three-line preset.
        var mrzFinderLayoutPreset = new SBSDKUI2ThreeLineMRZFinderLayoutPreset
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
        configuration.ViewFinder.Style = SBSDKUI2FinderStyle.FinderStrokedStyle;
        // For default cornered style.
        configuration.ViewFinder.Style = SBSDKUI2FinderStyle.FinderCorneredStyle;
        // You can also set each style's stroke width, stroke color or corner radius.
        // e.g
        configuration.ViewFinder.Style = new SBSDKUI2FinderCorneredStyle(new SBSDKUI2Color("#7A000000"), 3.0f, 2.0f);

        // Present the view controller modally.
        SBSDKUI2MRZScannerViewController.PresentOn(this, configuration, (controller, result, error) =>
        {
            if (result == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }

            // Handle the result.

            // Cast the resulted generic document to the MRZ model using the `wrap` method.
            var model = new SBSDKDocumentsModelMRZ(result.MrzDocument);
            if (model.BirthDate?.Value != null)
            {
                System.Console.WriteLine($"Birth date: {model.BirthDate.Value.Text}, Confidence: {model.BirthDate.ConfidenceWeight}");
            }
            if (model.Nationality?.Value != null)
            {
                System.Console.WriteLine($"Nationality: {model.Nationality.Value.Text}, Confidence: {model.Nationality.ConfidenceWeight}");
            }
        });
    }
}
