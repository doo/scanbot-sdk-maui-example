using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSDK.MAUI.Mrz;

namespace ScanbotSdkExample.Maui.Snippets.MrzScanner;

public class ScanningSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new MrzScannerScreenConfiguration();

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
        var scannedOutput = await ScanbotSdkMain.MrzScanner.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed mrz class.
        var mrz = new MRZ(scannedOutput.Result.MrzDocument);

        // Retrieve the values.
        // e.g
        Console.WriteLine($"Birth Date: {mrz.BirthDate.Value.Text}, Nationality: {mrz.Nationality.Value.Text}");
    }
}