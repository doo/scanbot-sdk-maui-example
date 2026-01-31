using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.CreditCardScanner;

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
        var configuration = new SBSDKUI2CreditCardScannerScreenConfiguration();

        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.TopUserGuidance = NSBundle.MainBundle.GetLocalizedString("top.user.guidance", "");
        localization.CameraPermissionCloseButton = NSBundle.MainBundle.GetLocalizedString("camera.permission.close", "");

        // Present the view controller modally.
        SBSDKUI2CreditCardScannerViewController.PresentOn(this, configuration, (result) =>
        {
            if (result == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }

            // Cast the resulted generic document to the credit card model.
            var model = new SBSDKCreditCardDocumentModelCreditCard(result.CreditCard);

            if (model.CardNumber?.Value != null)
            {
                Console.WriteLine($"Card number: {model.CardNumber.Value.Text}, Confidence: {model.CardNumber.ConfidenceWeight}");
            }

            if (model.CardholderName?.Value != null)
            {
                Console.WriteLine($"Name: {model.CardholderName.Value.Text}, Confidence: {model.CardholderName.ConfidenceWeight}");
            }
        });
    }
}