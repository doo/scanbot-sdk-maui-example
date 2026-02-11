using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.DocumentDataExtractor;

class LocalizationSnippet : UIViewController
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

        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.TopUserGuidance = NSBundle.MainBundle.GetLocalizedString("top.user.guidance", "");
        localization.CameraPermissionCloseButton = NSBundle.MainBundle.GetLocalizedString("camera.permission.close", "");

        // Present the view controller modally.
        SBSDKUI2DocumentDataExtractorViewController.PresentOn(this, configuration, (controller, result, error) =>
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