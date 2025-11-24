using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.CreditCardScanner;

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
        var configuration = new SBSDKUI2CreditCardScannerScreenConfiguration();

        // Configure user guidances

        // Top user guidance
        var topUserGuidance = configuration.TopUserGuidance;
        topUserGuidance.Visible = true;
        topUserGuidance.Title.Text = "Scan your Credit Card";
        topUserGuidance.Title.Color = new SBSDKUI2Color("#FFFFFF");
        topUserGuidance.Background.FillColor = new SBSDKUI2Color("#7A000000");

        // Scan status user guidance
        var scanStatusUserGuidance = configuration.ScanStatusUserGuidance;
        scanStatusUserGuidance.Visibility = true;
        scanStatusUserGuidance.Title.Text = "Scan credit card";
        scanStatusUserGuidance.Title.Color = new SBSDKUI2Color("#FFFFFF");
        scanStatusUserGuidance.Background.FillColor = new SBSDKUI2Color("#7A000000");

        // Present the view controller modally.
        SBSDKUI2CreditCardScannerViewController.PresentOn(this, configuration, (controller, result, error) =>
        {
            if (result == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }

            // Handle the result.
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
