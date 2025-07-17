using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.textpattern;

namespace ScanbotSdkExample.Maui.Snippets.TextPatternScanner;

public class ScanningSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new TextPatternScannerScreenConfiguration();

        // Configure camera properties.
        // e.g
        configuration.CameraConfiguration.ZoomSteps = [1.0, 2.0, 5.0];
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

        // Configure the success overlay.
        configuration.SuccessOverlay.IconColor = new ColorValue("#FFFFFF");
        configuration.SuccessOverlay.Message.Text = "Scanned Successfully!";
        configuration.SuccessOverlay.Message.Color = new ColorValue("#FFFFFF");

        // Configure the sound.
        configuration.Sound.SuccessBeepEnabled = true;
        configuration.Sound.SoundType = SoundType.ModernBeep;

        // Configure the vibration.
        configuration.Vibration.Enabled = false;

        // Present the view controller modally.
        var result = await ScanbotSDKMain.Rtu.TextPatternScanner.LaunchAsync(configuration);
       if (result.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }
       
         // Retrieve the value
        // e.g
        Console.WriteLine($"Scanned Text: "+ result.Result.RawText);
    }
}