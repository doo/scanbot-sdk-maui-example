using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.CreditCard;
using ScanbotSDK.MAUI.CreditCardDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CreditCardScanner;

public class ScanningSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new CreditCardScannerScreenConfiguration();
        
        // Configure the timeout for the scan process. If the scan process takes longer than this value, the
        // incomplete result will be returned.
        configuration.ScanIncompleteDataTimeout = 500;

        // Configure the success overlay.
        configuration.SuccessOverlay.Message.Text = "Scanned Successfully!";
        configuration.SuccessOverlay.IconColor = new ColorValue("#FFFFFF");
        configuration.SuccessOverlay.Message.Color = new ColorValue("#FFFFFF");
        // Set the timeout after which the overlay is dismissed.
        configuration.SuccessOverlay.Timeout = 100;

        // Configure the incomplete scan overlay.
        configuration.IncompleteDataOverlay.Message.Text = "Incomplete scan";
        configuration.IncompleteDataOverlay.IconColor = new ColorValue("#FFFFFF");
        configuration.IncompleteDataOverlay.Message.Color = new ColorValue("#FFFFFF");
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
        configuration.TopBarOpenIntroScreenButton.Color = new ColorValue("#FFFFFF");
        // Cancel button.
        configuration.TopBar.CancelButton.Visible = true;
        configuration.TopBar.CancelButton.Text = "Cancel";
        configuration.TopBar.CancelButton.Foreground.Color = new ColorValue("#FFFFFF");
        configuration.TopBar.CancelButton.Background.FillColor = new ColorValue("#00000000");

        // Configure the view finder.
        configuration.ViewFinder.Style = new FinderCorneredStyle
        {
            StrokeColor = new ColorValue("#7A000000"),
            CornerRadius = 3.0f,
            StrokeWidth = 2.0f
        };

        // Configure the action bar.
        configuration.ActionBar.FlashButton.Visible = true;
        configuration.ActionBar.ZoomButton.Visible = true;
        configuration.ActionBar.FlipCameraButton.Visible = false;

        // Configure the sound.
        configuration.Sound.SuccessBeepEnabled = true;
        configuration.Sound.SoundType = SoundType.ModernBeep;

        // Configure the vibration.
        configuration.Vibration.Enabled = false;
        
        // Present the view controller modally.
        var scannedOutput = await ScanbotSdkMain.CreditCardScanner.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed credit card.
        var creditCard = new CreditCard(scannedOutput.Result.CreditCard);
        
        // Retrieve the values.
        // e.g
        Console.WriteLine($"Card number: {creditCard.CardNumber.Value.Text}, Cardholder Name: {creditCard.CardholderName?.Value?.Text}");
    } 
}