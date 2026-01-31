using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.CreditCardScanner;

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
        var configuration = new SBSDKUI2CreditCardScannerScreenConfiguration();

        // Configure the timeout for the scan process. If the scan process takes longer than this value, the
        // incomplete result will be returned.
        configuration.ScanIncompleteDataTimeout = 500;

        // Configure the success overlay.
        configuration.SuccessOverlay.Message.Text = "Scanned Successfully!";
        configuration.SuccessOverlay.IconColor = new SBSDKUI2Color("#FFFFFF");
        configuration.SuccessOverlay.Message.Color = new SBSDKUI2Color("#FFFFFF");
        // Set the timeout after which the overlay is dismissed.
        configuration.SuccessOverlay.Timeout = 100;

        // Configure the incomplete scan overlay.
        configuration.IncompleteDataOverlay.Message.Text = "Incomplete scan";
        configuration.IncompleteDataOverlay.IconColor = new SBSDKUI2Color("#FFFFFF");
        configuration.IncompleteDataOverlay.Message.Color = new SBSDKUI2Color("#FFFFFF");
        // Set the timeout after which the overlay is dismissed.
        configuration.IncompleteDataOverlay.Timeout = 100;

        // Configure camera properties.
        // e.g
        configuration.CameraConfiguration.ZoomSteps = [ 1.0, 2.0, 3.0 ];
        configuration.CameraConfiguration.FlashEnabled = false;
        configuration.CameraConfiguration.PinchToZoomEnabled = true;

        // Configure the UI elements like icons or buttons.
        // e.g The top bar introduction button.
        configuration.TopBarOpenIntroScreenButton.Visible = true;
        configuration.TopBarOpenIntroScreenButton.Color = new SBSDKUI2Color("#FFFFFF");
        // Cancel button.
        configuration.TopBar.CancelButton.Visible = true;
        configuration.TopBar.CancelButton.Text = "Cancel";
        configuration.TopBar.CancelButton.Foreground.Color = new SBSDKUI2Color("#FFFFFF");
        configuration.TopBar.CancelButton.Background.FillColor = new SBSDKUI2Color("#00000000");

        // Configure the view finder.
        configuration.ViewFinder.Style = new SBSDKUI2FinderCorneredStyle(new SBSDKUI2Color("#7A000000"), 3.0f, 2.0f);

        // Configure the action bar.
        configuration.ActionBar.FlashButton.Visible = true;
        configuration.ActionBar.ZoomButton.Visible = true;
        configuration.ActionBar.FlipCameraButton.Visible = false;

        // Configure the sound.
        configuration.Sound.SuccessBeepEnabled = true;
        configuration.Sound.SoundType = SBSDKUI2SoundType.ModernBeep;

        // Configure the vibration.
        configuration.Vibration.Enabled = false;

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
