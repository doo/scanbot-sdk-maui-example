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

        // Set a validator
        configuration.ScannerConfiguration.Validator = new SBSDKPatternContentValidator
        {
            // Set a text pattern e.g. 4 digits
            Pattern = "^[0-9]{4}$",
            PatternGrammar = SBSDKPatternGrammar.Regex,
            MatchSubstring = true
        };

        // Present the view controller modally.
        SBSDKUI2TextPatternScannerViewController.PresentOn(this, configuration, completion: (controller, result, error) =>
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