using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.VinScanner;

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
        var configuration = new SBSDKUI2VINScannerScreenConfiguration();

        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new SBSDKUI2Color("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan a VIN";

        // Configure the image for the introduction screen.
        configuration.IntroScreen.Image = SBSDKUI2VINScannerIntroImage.VinIntroNoImage;
        
        // For a custom image...
        configuration.IntroScreen.Image = SBSDKUI2VINScannerIntroImage.VinIntroCustomImageWithUri("PathToImage");
        
        // Or you can also use our default image.
        configuration.IntroScreen.Image = SBSDKUI2VINScannerIntroImage.VinIntroDefaultImage;

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new SBSDKUI2Color("#000000");
        configuration.IntroScreen.Explanation.Text =
            "To scan a VIN (Vehicle Identification Number), please hold your device so that the camera viewfinder clearly captures the VIN code. Please ensure the VIN is properly aligned. Once the scan is complete, the VIN will be automatically extracted.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new SBSDKUI2Color("#C8193C");

        // Present the view controller modally.
        SBSDKUI2VINScannerViewController.PresentOn(this, configuration, (controller, result, error) =>
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