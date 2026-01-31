using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.VinScanner;

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
        var configuration = new SBSDKUI2VINScannerScreenConfiguration();

        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.TopUserGuidance = NSBundle.MainBundle.GetLocalizedString("top.user.guidance", "");
        localization.CameraPermissionCloseButton = NSBundle.MainBundle.GetLocalizedString("camera.permission.close", "");

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