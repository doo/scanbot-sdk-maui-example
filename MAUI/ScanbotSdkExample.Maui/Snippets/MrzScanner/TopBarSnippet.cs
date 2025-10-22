using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSDK.MAUI.Mrz;

namespace ScanbotSdkExample.Maui.Snippets.MrzScanner;

public class TopBarSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new MrzScannerScreenConfiguration();

        // Set the top bar mode.
        configuration.TopBar.Mode = TopBarMode.Gradient;

        // Set the background color which will be used as a gradient.
        configuration.TopBar.BackgroundColor = new ColorValue("#C8193C");

        // Set the status bar mode.
        configuration.TopBar.StatusBarMode = StatusBarMode.Light;

        // Configure the cancel button.
        configuration.TopBar.CancelButton.Text = "Cancel";
        configuration.TopBar.CancelButton.Foreground.Color = new ColorValue("#FFFFFF");
        
        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.Rtu.MrzScanner.LaunchAsync(configuration);
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