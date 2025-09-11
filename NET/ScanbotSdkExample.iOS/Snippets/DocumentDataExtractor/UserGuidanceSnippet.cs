using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.DocumentDataExtractor;

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
        var configuration = new SBSDKUI2DocumentDataExtractorScreenConfiguration();

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

        // Scan status user guidance
        // Retrieve the instance of the user guidance from the configuration object.
        var scanStatusUserGuidance = configuration.ScanStatusUserGuidance;
        // Show the user guidance.
        scanStatusUserGuidance.Visibility = true;
        // Configure the title.
        scanStatusUserGuidance.Title.Text = "How to scan an ID document";
        scanStatusUserGuidance.Title.Color = new SBSDKUI2Color(colorString: "#FFFFFF");
        // Configure the background.
        scanStatusUserGuidance.Background.FillColor = new SBSDKUI2Color(colorString: "#7A000000");
            
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
