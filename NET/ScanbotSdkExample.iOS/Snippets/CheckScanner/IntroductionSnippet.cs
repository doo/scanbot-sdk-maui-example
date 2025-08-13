using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.CheckScanner;

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
        var configuration = new SBSDKUI2CheckScannerScreenConfiguration();

        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new SBSDKUI2Color("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan a check";

        // Configure the image for the introduction screen.
        configuration.IntroScreen.Image = SBSDKUI2CheckScannerIntroImage.CheckNoImage;
        
        // For a custom image...
        configuration.IntroScreen.Image = SBSDKUI2CheckScannerIntroImage.CheckIntroCustomImageWithUri("PathToImage");
        
        // Or you can also use our default image.
        configuration.IntroScreen.Image = SBSDKUI2CheckScannerIntroImage.CheckIntroDefaultImage;

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new SBSDKUI2Color("#000000");
        configuration.IntroScreen.Explanation.Text =
            "To quickly and securely scan your check details, please hold your device over the check, so that the camera aligns with all the information on the check.\n\nThe scanner will guide you to the optimal scanning position. Once the scan is complete, your check details will automatically be extracted and processed.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new SBSDKUI2Color("#C8193C");

        // Present the view controller modally.
        SBSDKUI2CheckScannerViewController.PresentOn(this, configuration, (result) =>
        {
            if (result == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }
            
            // Wrap the resulted generic document to the strongly typed check.
            var check = new SBSDKCheckDocumentModelUSACheck(result.Check);
           
            // Retrieve the values.
            Console.WriteLine($"Account number: {check.AccountNumber?.Value?.Text}");
            Console.WriteLine($"Transit Number: {check.TransitNumber?.Value?.Text}");
            Console.WriteLine($"AuxiliaryOnUs: {check.AuxiliaryOnUs?.Value?.Text}");
        });
    }
}