using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.MRZScanner;

class ActionBarSnippet : UIViewController
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

        // Retrieve the instance of the action bar from the configuration object.
        var actionBar = configuration.ActionBar;

        // Show the flash button.
        actionBar.FlashButton.Visible = true;

        // Configure the inactive state of the flash button.
        actionBar.FlashButton.BackgroundColor = new SBSDKUI2Color("#7A000000");
        actionBar.FlashButton.ForegroundColor = new SBSDKUI2Color("#FFFFFF");

        // Configure the active state of the flash button.
        actionBar.FlashButton.ActiveBackgroundColor = new SBSDKUI2Color("#FFCE5C");
        actionBar.FlashButton.ActiveForegroundColor = new SBSDKUI2Color("#000000");

        // Show the zoom button.
        actionBar.ZoomButton.Visible = true;

        // Configure the zoom button.
        actionBar.ZoomButton.BackgroundColor = new SBSDKUI2Color("#7A000000");
        actionBar.ZoomButton.ForegroundColor = new SBSDKUI2Color("#FFFFFF");

        // Show the flip camera button.
        actionBar.FlipCameraButton.Visible = true;

        // Configure the flip camera button.
        actionBar.FlipCameraButton.BackgroundColor = new SBSDKUI2Color("#7A000000");
        actionBar.FlipCameraButton.ForegroundColor = new SBSDKUI2Color("#FFFFFF");

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
                Console.WriteLine($"Birth date: {model.BirthDate.Value.Text}, Confidence: {model.BirthDate.ConfidenceWeight}");
            }

            if (model.Nationality?.Value != null)
            {
                Console.WriteLine($"Nationality: {model.Nationality.Value.Text}, Confidence: {model.Nationality.ConfidenceWeight}");
            }
        });
    }
}
