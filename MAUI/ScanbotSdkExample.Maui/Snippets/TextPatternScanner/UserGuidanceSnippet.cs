using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.TextPattern;

namespace ScanbotSdkExample.Maui.Snippets.TextPatternScanner;

public class UserGuidanceSnippet
{
    public static async Task StartScannerAsync()
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
        var result = await ScanbotSDKMain.TextPattern.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }

        // Retrieve the value
        // e.g
        Console.WriteLine($"Scanned Text: " + result.Value.RawText);
    }
}