using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.textpattern;

namespace ScanbotSdkExample.Maui.Snippets.TextPatternScanner;

public class UserGuidanceSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new TextPatternScannerScreenConfiguration();

        // Configure user guidances

        // Top user guidance
        // Retrieve the instance of the top user guidance from the configuration object.
        var topUserGuidance = configuration.TopUserGuidance;
        // Show the user guidance.
        topUserGuidance.Visible = true;
        // Configure the title.
        topUserGuidance.Title.Text = "Locate the text you are looking for";
        topUserGuidance.Title.Color = new ColorValue("#FFFFFF");
        // Configure the background.
        topUserGuidance.Background.FillColor = new ColorValue("#7A000000");

        // Finder overlay user guidance
        // Retrieve the instance of the finder overlay user guidance from the configuration object.
        var finderUserGuidance = configuration.FinderViewUserGuidance;
        // Show the user guidance.
        finderUserGuidance.Visible = true;
        // Configure the title.
        finderUserGuidance.Title.Text = "Scanning for text pattern...";
        finderUserGuidance.Title.Color = new ColorValue("#FFFFFF");
        // Configure the background.
        finderUserGuidance.Background.FillColor = new ColorValue("#7A000000");

        // Present the view controller modally.
        var result = await ScanbotSDKMain.Rtu.TextPatternScanner.LaunchAsync(configuration);
       if (result?.Result?.RawText == null)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }
       
         // Retrieve the value
        // e.g
        Console.WriteLine($"Scanned Text: "+ result.Result.RawText);
    }
}