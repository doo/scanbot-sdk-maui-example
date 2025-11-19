using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.DocumentData;

namespace ScanbotSdkExample.Maui.Snippets.DocumentDataExtractor;

public class ScanningSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentDataExtractorScreenConfiguration();
        
        // Configure the timeout for the document data extractor to wait for a document to be found.
        // If no document is found within this time, the warning alert will be shown.
        configuration.NoDocumentFoundTimeout = 1000;
        
        // Configure the timeout for the scan process.
        // If the scan process takes longer than this value, the warning alert will be shown.
        configuration.AccumulationTimeout = 500;

        // Configure the success overlay.
        configuration.SuccessOverlay.Message.Text = "Scanned Successfully!";
        configuration.SuccessOverlay.IconColor = new ColorValue("#FFFFFF");
        configuration.SuccessOverlay.Message.Color = new ColorValue("#FFFFFF");
        // Set the timeout after which the overlay is dismissed.
        configuration.SuccessOverlay.Timeout = 100;

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
        var scannedOutput = await ScanbotSdkMain.DocumentDataExtractor.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }
        
        // Iterate through all the document fields
        foreach (var field in scannedOutput.Result.Document.Fields)
        {
            Console.WriteLine($"{field.Type.Name}: {field.Value.Text}");
        }
    } 
}