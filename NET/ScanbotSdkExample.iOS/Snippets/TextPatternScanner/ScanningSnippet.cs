using Foundation;
using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.TextPatternScanner;

class ScanningSnippet : UIViewController
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

        // Configure camera properties.
        configuration.CameraConfiguration.ZoomSteps = [ 1.0, 2.0, 5.0 ];
        configuration.CameraConfiguration.FlashEnabled = false;
        configuration.CameraConfiguration.PinchToZoomEnabled = true;

        // Configure the UI elements like icons or buttons.
        configuration.TopBarOpenIntroScreenButton.Visible = true;
        configuration.TopBarOpenIntroScreenButton.Color = new SBSDKUI2Color("#FFFFFF");

        // Cancel button.
        configuration.TopBar.CancelButton.Visible = true;
        configuration.TopBar.CancelButton.Text = "Cancel";
        configuration.TopBar.CancelButton.Foreground.Color = new SBSDKUI2Color("#FFFFFF");
        configuration.TopBar.CancelButton.Background.FillColor = new SBSDKUI2Color("#00000000");

        // Configure the view finder.
        configuration.ViewFinder.AspectRatio = new SBSDKAspectRatio(3.85, 1.0);

        // Configure the style for the view finder.
        // For default stroked style.
        configuration.ViewFinder.Style = SBSDKUI2FinderStyle.FinderStrokedStyle;
        // For default cornered style.
        configuration.ViewFinder.Style = SBSDKUI2FinderStyle.FinderCorneredStyle;

        // You can also set each style's stroke width, stroke color or corner radius.
        configuration.ViewFinder.Style = new SBSDKUI2FinderCorneredStyle(new SBSDKUI2Color("#7A000000"), 3.0f, 2.0f);

        // Configure the success overlay.
        configuration.SuccessOverlay.IconColor = new SBSDKUI2Color("#FFFFFF");
        configuration.SuccessOverlay.Message.Text = "Scanned Successfully!";
        configuration.SuccessOverlay.Message.Color = new SBSDKUI2Color("#FFFFFF");

        // Configure the sound.
        configuration.Sound.SuccessBeepEnabled = true;
        configuration.Sound.SoundType = SBSDKUI2SoundType.ModernBeep;

        // Configure the vibration.
        configuration.Vibration.Enabled = false;

        // Present the view controller modally.
        SBSDKUI2TextPatternScannerViewController.PresentOn(presenter: this, configuration: configuration, completion: (controller, result, error) =>
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
