using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.VinScanner;

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
        var configuration = new SBSDKUI2VINScannerScreenConfiguration();

        // Configure user guidances

        // Top user guidance
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
        finderUserGuidance.Title.Text = "Scanning for VIN...";
        finderUserGuidance.Title.Color = new SBSDKUI2Color(colorString: "#FFFFFF");
        // Configure the background.
        finderUserGuidance.Background.FillColor = new SBSDKUI2Color(colorString: "#7A000000");
            
        // Present the view controller modally.
        SBSDKUI2VINScannerViewController.PresentOn(this, configuration, (result) =>
        {
            if (result == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }
            
            // Handle the result
            Console.WriteLine($"Vin Scanner result: {result.TextResult.RawText}");
        });
    }
}
