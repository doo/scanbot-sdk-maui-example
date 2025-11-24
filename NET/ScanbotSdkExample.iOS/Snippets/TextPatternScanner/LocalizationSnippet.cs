using Foundation;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.TextPatternScanner;

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
        var configuration = new SBSDKUI2TextPatternScannerScreenConfiguration();

        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        // e.g
        localization.TopUserGuidance = NSBundle.MainBundle.GetLocalizedString("top.user.guidance", "");
        localization.CameraPermissionCloseButton = NSBundle.MainBundle.GetLocalizedString("camera.permission.close", "");

        // Present the view controller modally.
        SBSDKUI2TextPatternScannerViewController.PresentOn(presenter: this, configuration: configuration, completion: (controller, result, error) =>
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