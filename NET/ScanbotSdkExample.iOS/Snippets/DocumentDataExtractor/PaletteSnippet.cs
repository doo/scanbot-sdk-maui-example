using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.DocumentDataExtractor;

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
        var configuration = new SBSDKUI2DocumentDataExtractorScreenConfiguration();

        // Retrieve the instance of the palette from the configuration object.
        var palette = configuration.Palette;

        // Configure the colors.
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
        SBSDKUI2DocumentDataExtractorViewController.PresentOn(this, configuration, (result) =>
        {
            if (result?.Document == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }
            
            // Iterate through all the document fields
            foreach (var field in result.Document.Fields)
            {
                Console.WriteLine($"{field.Type.Name}: {field.Value?.Text}");
            }
        });
    }
}
