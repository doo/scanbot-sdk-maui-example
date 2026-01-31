using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.TextPatternScanner;

class LaunchSnippet : UIViewController
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