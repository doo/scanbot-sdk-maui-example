using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.CreditCardScanner;

class TopBarSnippet : UIViewController
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

        // Set the top bar mode.
        configuration.TopBar.Mode = SBSDKUI2TopBarMode.Gradient;

        // Set the background color which will be used as a gradient.
        configuration.TopBar.BackgroundColor = new SBSDKUI2Color("#C8193C");

        // Set the status bar mode.
        configuration.TopBar.StatusBarMode = SBSDKUI2StatusBarMode.Light;

        // Configure the cancel button.
        configuration.TopBar.CancelButton.Text = "Cancel";
        configuration.TopBar.CancelButton.Foreground.Color = new SBSDKUI2Color("#FFFFFF");

        // Present the view controller modally.
        SBSDKUI2CreditCardScannerViewController.PresentOn(this, configuration, (result) =>
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