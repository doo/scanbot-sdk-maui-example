using Foundation;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.TextPatternScanner;

class UserGuidanceSnippet : UIViewController
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

        // Configure user guidances

        // Top user guidance
        // Retrieve the instance of the top user guidance from the configuration object.
        var topUserGuidance = configuration.TopUserGuidance;
        // Show the user guidance.
        topUserGuidance.Visible = true;
        // Configure the title.
        topUserGuidance.Title.Text = "Locate the text you are looking for";
        topUserGuidance.Title.Color = new SBSDKUI2Color("#FFFFFF");
        // Configure the background.
        topUserGuidance.Background.FillColor = new SBSDKUI2Color("#7A000000");

        // Finder overlay user guidance
        // Retrieve the instance of the finder overlay user guidance from the configuration object.
        var finderUserGuidance = configuration.FinderViewUserGuidance;
        // Show the user guidance.
        finderUserGuidance.Visible = true;
        // Configure the title.
        finderUserGuidance.Title.Text = "Scanning for text pattern...";
        finderUserGuidance.Title.Color = new SBSDKUI2Color("#FFFFFF");
        // Configure the background.
        finderUserGuidance.Background.FillColor = new SBSDKUI2Color("#7A000000");

        // Present the view controller modally.
        SBSDKUI2TextPatternScannerViewController.PresentOn(this, configuration, completion: (controller, result, error)  =>
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
