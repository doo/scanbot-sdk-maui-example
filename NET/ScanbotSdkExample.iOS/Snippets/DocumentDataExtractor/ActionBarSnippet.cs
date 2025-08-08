using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.DocumentDataExtractor;

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
        var configuration = new SBSDKUI2DocumentDataExtractorScreenConfiguration();

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