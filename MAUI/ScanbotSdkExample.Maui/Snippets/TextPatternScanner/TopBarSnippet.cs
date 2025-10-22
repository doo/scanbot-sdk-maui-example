using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.TextPattern;

namespace ScanbotSdkExample.Maui.Snippets.TextPatternScanner;

public class TopBarSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new TextPatternScannerScreenConfiguration();

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
        var scannedOutput = await ScanbotSDKMain.Rtu.TextPatternScanner.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Retrieve the value
        // e.g
        Console.WriteLine($"Scanned Text: " + scannedOutput.Result.RawText);
    }
}