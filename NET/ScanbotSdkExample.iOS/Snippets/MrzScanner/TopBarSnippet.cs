using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.MRZScanner;

class TopBarSnippet : UIViewController
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

        // Set the top bar mode.
        configuration.TopBar.Mode = SBSDKUI2TopBarMode.Gradient;

        // Set the background color which will be used as a gradient.
        configuration.TopBar.BackgroundColor = new SBSDKUI2Color("#C8193C");

        // Set the status bar mode.
        configuration.TopBar.StatusBarMode = SBSDKUI2StatusBarMode.Light;

        // Configure the cancel button.
        configuration.TopBar.CancelButton.Text = "Cancel";
        configuration.TopBar.CancelButton.Foreground.Color = new SBSDKUI2Color("#FFFFFF");

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