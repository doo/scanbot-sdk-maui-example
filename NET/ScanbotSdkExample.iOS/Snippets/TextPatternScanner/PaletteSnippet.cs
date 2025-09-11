using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.TextPatternScanner;

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
        var configuration = new SBSDKUI2TextPatternScannerScreenConfiguration();

        // Retrieve the instance of the palette from the configuration object.
        var palette = configuration.Palette;

        // Configure the colors.
        // The palette already has the default colors set, so you don't have to always set all the colors.
        palette.SbColorPrimary = new SBSDKUI2Color(colorString: "#C8193C");
        palette.SbColorPrimaryDisabled = new SBSDKUI2Color(colorString: "#F5F5F5");
        palette.SbColorNegative = new SBSDKUI2Color(colorString: "#FF3737");
        palette.SbColorPositive = new SBSDKUI2Color(colorString: "#4EFFB4");
        palette.SbColorWarning = new SBSDKUI2Color(colorString: "#FFCE5C");
        palette.SbColorSecondary = new SBSDKUI2Color(colorString: "#FFEDEE");
        palette.SbColorSecondaryDisabled = new SBSDKUI2Color(colorString: "#F5F5F5");
        palette.SbColorOnPrimary = new SBSDKUI2Color(colorString: "#FFFFFF");
        palette.SbColorOnSecondary = new SBSDKUI2Color(colorString: "#C8193C");
        palette.SbColorSurface = new SBSDKUI2Color(colorString: "#FFFFFF");
        palette.SbColorOutline = new SBSDKUI2Color(colorString: "#EFEFEF");
        palette.SbColorOnSurfaceVariant = new SBSDKUI2Color(colorString: "#707070");
        palette.SbColorOnSurface = new SBSDKUI2Color(colorString: "#000000");
        palette.SbColorSurfaceLow = new SBSDKUI2Color(colorString: "#26000000");
        palette.SbColorSurfaceHigh = new SBSDKUI2Color(colorString: "#7A000000");
        palette.SbColorModalOverlay = new SBSDKUI2Color(colorString: "#A3000000");

        // Present the view controller modally.
        SBSDKUI2TextPatternScannerViewController.PresentOn(this, configuration, result =>
        {
            if (result != null)
            {
                // Handle the result.

            }
            else
            {
                // Indicates that the cancel button was tapped.
            }
        });
    }
}
