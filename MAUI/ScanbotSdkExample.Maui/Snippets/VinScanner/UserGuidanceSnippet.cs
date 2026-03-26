using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Vin;

namespace ScanbotSdkExample.Maui.Snippets.VinScanner;

public class UserGuidanceSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new VinScannerScreenConfiguration();
        
        // Set the top bar mode.
        configuration.TopBar.Mode = TopBarMode.Gradient;

        // Set the background color which will be used as a gradient.
        configuration.TopBar.BackgroundColor = new ColorValue("#C8193C");

        // Set the status bar mode.
        configuration.TopBar.StatusBarMode = StatusBarMode.Light;

        // Configure the cancel button.
        configuration.TopBar.CancelButton.Text = "Cancel";
        configuration.TopBar.CancelButton.Foreground.Color = new ColorValue("#FFFFFF");
        
        // Finder overlay user guidance
        // Retrieve the instance of the finder overlay user guidance from the configuration object.
        // Show the user guidance.
        configuration.FinderViewUserGuidance.Visible = true;
        
        // Configure the title.
        configuration.FinderViewUserGuidance.Title.Text = "Scanning for VIN...";
        configuration.FinderViewUserGuidance.Title.Color = new ColorValue( "#FFFFFF");
        
        // Configure the background.
        configuration.FinderViewUserGuidance.Background.FillColor = new ColorValue( "#7A000000");
        
        // Present the view controller modally.
        var result = await ScanbotSDKMain.Vin.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }

        // Print the scanned text results
        Console.WriteLine($"Scanned Vin Scanner: {result.Value.TextResult?.RawText}");
    } 
}