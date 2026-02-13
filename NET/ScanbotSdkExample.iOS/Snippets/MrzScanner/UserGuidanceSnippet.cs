using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.MRZScanner;

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
        var configuration = new SBSDKUI2MRZScannerScreenConfiguration();

        // Configure user guidances

        // Top user guidance
        // Retrieve the instance of the top user guidance from the configuration object.
        var topUserGuidance = configuration.TopUserGuidance;
        // Show the user guidance.
        topUserGuidance.Visible = true;
        // Configure the title.
        topUserGuidance.Title.Text = "Scan your Identity Document";
        topUserGuidance.Title.Color = new SBSDKUI2Color("#FFFFFF");
        // Configure the background.
        topUserGuidance.Background.FillColor = new SBSDKUI2Color("#7A000000");

        // Finder overlay user guidance
        // Retrieve the instance of the finder overlay user guidance from the configuration object.
        var finderUserGuidance = configuration.FinderViewUserGuidance;
        // Show the user guidance.
        finderUserGuidance.Visible = true;
        // Configure the title.
        finderUserGuidance.Title.Text = "Scan the MRZ";
        finderUserGuidance.Title.Color = new SBSDKUI2Color("#FFFFFF");
        // Configure the background.
        finderUserGuidance.Background.FillColor = new SBSDKUI2Color("#7A000000");

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
