using ScanbotSDK.iOS;
using Foundation;

namespace ScanbotSdkExample.iOS.Snippets.MRZScanner;

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
        var configuration = new SBSDKUI2MRZScannerScreenConfiguration();

        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new SBSDKUI2Color("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan an MRZ";

        // Configure the image for the introduction screen.
        // If you want to have no image...
        configuration.IntroScreen.Image = SBSDKUI2MrzScannerIntroImage.MrzIntroNoImage;
        // For a custom image...
        configuration.IntroScreen.Image = SBSDKUI2MrzScannerIntroImage.MrzIntroCustomImageWithUri("PathToImage");
        // Or you can also use our default image.
        configuration.IntroScreen.Image = SBSDKUI2MrzScannerIntroImage.MrzIntroDefaultImage;

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new SBSDKUI2Color("#000000");
        configuration.IntroScreen.Explanation.Text = "The Machine Readable Zone (MRZ) is a special code on your ID document (such as a passport or ID card) that contains your personal information in a machine-readable format.\n\nTo scan it, simply hold your camera over the document, so that it aligns with the MRZ section. Once scanned, the data will be automatically processed, and you will be directed to the results screen.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        // e.g the text or the background color.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new SBSDKUI2Color("#C8193C");

        // Present the view controller modally.
        SBSDKUI2MRZScannerViewController.PresentOn(this, configuration, (result) =>
        {
            if (result == null)
            {
                // Indicates that the cancel button was tapped.
                return;
            }

            // Handle the result.

            // Cast the resulted generic document to the MRZ model using the `wrap` method.
            var model = new SBSDKDocumentsModelMRZ(result.MrzDocument);
            if (model.BirthDate?.Value != null)
            {
                Console.WriteLine($"Birth date: {model.BirthDate.Value.Text}, Confidence: {model.BirthDate.ConfidenceWeight}");
            }
            if (model.Nationality?.Value != null)
            {
                Console.WriteLine($"Nationality: {model.Nationality.Value.Text}, Confidence: {model.Nationality.ConfidenceWeight}");
            }
        });
    }
}
