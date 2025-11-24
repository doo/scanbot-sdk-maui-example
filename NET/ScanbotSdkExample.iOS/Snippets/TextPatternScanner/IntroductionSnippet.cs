using Foundation;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.TextPatternScanner;

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
        var configuration = new SBSDKUI2TextPatternScannerScreenConfiguration();

        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new SBSDKUI2Color("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan text";

        // Configure the image for the introduction screen.
        // If you want to have no image...
        configuration.IntroScreen.Image = SBSDKUI2TextPatternScannerIntroImage.TextPatternIntroNoImage;
        // For a custom image...
        configuration.IntroScreen.Image = SBSDKUI2TextPatternScannerIntroImage.TextPatternIntroCustomImageWithUri("PathToImage");
        // Or you can also use one of our default images.
        // e.g the meter device image.
        configuration.IntroScreen.Image = SBSDKUI2TextPatternScannerIntroImage.TextPatternIntroMeterDevice;
        // shipping container image.
        configuration.IntroScreen.Image = SBSDKUI2TextPatternScannerIntroImage.TextPatternIntroShippingContainer;
        // general text field image.
        configuration.IntroScreen.Image = SBSDKUI2TextPatternScannerIntroImage.TextPatternIntroGeneralField;
        // alphabetic text field image.
        configuration.IntroScreen.Image = SBSDKUI2TextPatternScannerIntroImage.TextPatternIntroAlphabeticField;

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new SBSDKUI2Color("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new SBSDKUI2Color("#000000");
        configuration.IntroScreen.Explanation.Text = "To scan a single line of text, please hold your device so that the camera viewfinder clearly captures the text you want to scan. Please ensure the text is properly aligned. Once the scan is complete, the text will be automatically extracted.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        // e.g the text or the background color.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new SBSDKUI2Color("#C8193C");

        // Present the view controller modally.
        SBSDKUI2TextPatternScannerViewController.PresentOn(this, configuration, (controller, result, error) =>
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
