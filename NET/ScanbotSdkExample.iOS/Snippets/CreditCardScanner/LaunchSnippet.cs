using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.CreditCardScanner;

class LaunchSnippet: UIViewController {
    
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

        // Present the view controller modally.
        SBSDKUI2CreditCardScannerViewController.PresentOn(this, configuration, (result) =>
        {
            if (result == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }

            // Handle the result.
            // Cast the generic document to a strongly typed SBSDKCreditCardDocumentModelCreditCard.
            
            var model = new SBSDKCreditCardDocumentModelCreditCard(result.CreditCard);
            // Retrieve the values.
            if (model.CardNumber?.Value != null)
            {
                Console.WriteLine(
                    $"Card number: {model.CardNumber.Value.Text}, Confidence: {model.CardNumber.ConfidenceWeight}");
            }

            if (model.CardholderName?.Value != null)
            {
                Console.WriteLine(
                    $"Name: {model.CardholderName.Value.Text}, Confidence: {model.CardholderName.ConfidenceWeight}");
            }
        });
    }
}