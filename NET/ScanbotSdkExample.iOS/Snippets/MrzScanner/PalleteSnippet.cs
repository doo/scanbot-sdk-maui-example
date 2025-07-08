using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.MRZScanner;

class PaletteSnippet : UIViewController
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

        // Retrieve the instance of the palette from the configuration object.
        var palette = configuration.Palette;

        // Configure the colors.
        // The palette already has the default colors set, so you don't have to always set all the colors.
        palette.SbColorPrimary = new SBSDKUI2Color("#C8193C");
        palette.SbColorPrimaryDisabled = new SBSDKUI2Color("#F5F5F5");
        palette.SbColorNegative = new SBSDKUI2Color("#FF3737");
        palette.SbColorPositive = new SBSDKUI2Color("#4EFFB4");
        palette.SbColorWarning = new SBSDKUI2Color("#FFCE5C");
        palette.SbColorSecondary = new SBSDKUI2Color("#FFEDEE");
        palette.SbColorSecondaryDisabled = new SBSDKUI2Color("#F5F5F5");
        palette.SbColorOnPrimary = new SBSDKUI2Color("#FFFFFF");
        palette.SbColorOnSecondary = new SBSDKUI2Color("#C8193C");
        palette.SbColorSurface = new SBSDKUI2Color("#FFFFFF");
        palette.SbColorOutline = new SBSDKUI2Color("#EFEFEF");
        palette.SbColorOnSurfaceVariant = new SBSDKUI2Color("#707070");
        palette.SbColorOnSurface = new SBSDKUI2Color("#000000");
        palette.SbColorSurfaceLow = new SBSDKUI2Color("#26000000");
        palette.SbColorSurfaceHigh = new SBSDKUI2Color("#7A000000");
        palette.SbColorModalOverlay = new SBSDKUI2Color("#A3000000");

        // Present the view controller modally.
        SBSDKUI2MRZScannerViewController.PresentOn(this, configuration, (result) =>
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
                Console.WriteLine($"Birth date: {model.BirthDate.Value.Text}, Confidence: {model.BirthDate.ConfidenceWeight}");
            }
            if (model.Nationality?.Value != null)
            {
                Console.WriteLine($"Nationality: {model.Nationality.Value.Text}, Confidence: {model.Nationality.ConfidenceWeight}");
            }
        });
    }
}
