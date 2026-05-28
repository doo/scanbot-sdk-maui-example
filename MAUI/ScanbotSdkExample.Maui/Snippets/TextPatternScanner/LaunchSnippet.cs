using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.TextPattern;
using ScanbotSDK.MAUI.TextPattern;

namespace ScanbotSdkExample.Maui.Snippets.TextPatternScanner;

public class LaunchSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new TextPatternScannerScreenConfiguration();

        // Set a validator
        configuration.ScannerConfiguration.Validator = new PatternContentValidator
        {
            // Set a text pattern e.g. 4 digits
            Pattern = "^[0-9]{4}$",
            PatternGrammar = PatternGrammar.Regex,
            MatchSubstring = true
        };

        // Present the view controller modally.
        var result = await ScanbotSDKMain.TextPattern.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }

        // Retrieve the value.
        // e.g
        Console.WriteLine($"Scanned Text: " + result.Value.RawText);
    }
}