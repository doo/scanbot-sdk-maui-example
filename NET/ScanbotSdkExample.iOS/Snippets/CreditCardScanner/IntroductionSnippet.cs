using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.CreditCardScanner;

class IntroductionSnippet : UIViewController
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

        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new SBSDKUI2Color("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan a credit card";

        // Configure the image for the introduction screen.
        configuration.IntroScreen.Image = SBSDKUI2CreditCardScannerIntroImage.CreditCardNoImage;
        
        // For a custom image...
        configuration.IntroScreen.Image = SBSDKUI2CreditCardScannerIntroImage.CreditCardIntroCustomImageWithUri("PathToImage");
        
        // Or you can also use our default one side image.
        configuration.IntroScreen.Image = SBSDKUI2CreditCardScannerIntroImage.CreditCardIntroOneSideImage;
        
        // Or you can also use our default two sides image.
        configuration.IntroScreen.Image = SBSDKUI2CreditCardScannerIntroImage.CreditCardIntroTwoSidesImage;

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new SBSDKUI2Color("#000000");
        configuration.IntroScreen.Explanation.Text =
            "To quickly and securely input your credit card details, please hold your device over the credit card, so that the camera aligns with the numbers on the front of the card.\n\n" +
            "The scanner will guide you to the optimal scanning position. Once the scan is complete, your card details will automatically be extracted and processed.\n\n" +
            "Press 'Start Scanning' to begin.";

        // Configure the done button.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new SBSDKUI2Color("#C8193C");

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