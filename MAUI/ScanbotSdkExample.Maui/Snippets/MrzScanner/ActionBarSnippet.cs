using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSDK.MAUI.MRZ;

namespace ScanbotSdkExample.Maui.Snippets.MrzScanner;

public class ActionBarSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new MrzScannerScreenConfiguration();

        // Retrieve the instance of the action bar from the configuration object.
        var actionBar = configuration.ActionBar;

        // Show the flash button.
        actionBar.FlashButton.Visible = true;

        // Configure the inactive state of the flash button.
        actionBar.FlashButton.BackgroundColor = new ColorValue("#7A000000");
        actionBar.FlashButton.ForegroundColor = new ColorValue("#FFFFFF");

        // Configure the active state of the flash button.
        actionBar.FlashButton.ActiveBackgroundColor = new ColorValue("#FFCE5C");
        actionBar.FlashButton.ActiveForegroundColor = new ColorValue("#000000");

        // Show the zoom button.
        actionBar.ZoomButton.Visible = true;

        // Configure the zoom button.
        actionBar.ZoomButton.BackgroundColor = new ColorValue("#7A000000");
        actionBar.ZoomButton.ForegroundColor = new ColorValue("#FFFFFF");

        // Show the flip camera button.
        actionBar.FlipCameraButton.Visible = true;

        // Configure the flip camera button.
        actionBar.FlipCameraButton.BackgroundColor = new ColorValue("#7A000000");
        actionBar.FlipCameraButton.ForegroundColor = new ColorValue("#FFFFFF");

        // Present the view controller modally.
        var result = await ScanbotSDKMain.Rtu.MrzScanner.LaunchAsync(configuration);
        if (result?.Result?.MrzDocument == null)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed Mrz class.
        var mrz = new MRZ(result.Result.MrzDocument);

        // Retrieve the values.
        // e.g
        Console.WriteLine($"Birth Date: {mrz.BirthDate.Value.Text}, Nationality: {mrz.Nationality.Value.Text}");
    }
}