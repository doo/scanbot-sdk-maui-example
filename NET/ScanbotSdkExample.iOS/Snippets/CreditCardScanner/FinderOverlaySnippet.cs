using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.CreditCardScanner;

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
        var configuration = new SBSDKUI2CreditCardScannerScreenConfiguration();

        // Set the example overlay visibility.
        configuration.ExampleOverlayVisible = true;

        // Configure the view finder.
        // Set the style for the view finder.
        // Choose between cornered or stroked style.
        // For default stroked style.
        configuration.ViewFinder.Style = SBSDKUI2FinderStyle.FinderStrokedStyle;
        // For default cornered style.
        configuration.ViewFinder.Style = SBSDKUI2FinderStyle.FinderCorneredStyle;
        // You can also set each style's stroke width, stroke color or corner radius.
        // e.g
        configuration.ViewFinder.Style = new SBSDKUI2FinderStrokedStyle(new SBSDKUI2Color("#7A000000"), 3.0f, 2.0f);

        // Present the view controller modally.
        SBSDKUI2CreditCardScannerViewController.PresentOn(this, configuration, (result) =>
        {
            if (result == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }

            // Handle the result here.
        });
    }
}