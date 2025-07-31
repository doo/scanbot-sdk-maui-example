using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.TextPattern;

namespace ScanbotSdkExample.Maui.Snippets.TextPatternScanner;

public class IntroductionSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new TextPatternScannerScreenConfiguration();

        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new ColorValue("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan text";

        // Configure the image for the introduction screen.
        // If you want to have no image...
        configuration.IntroScreen.Image = new TextPatternIntroNoImage();
        // For a custom image...
        configuration.IntroScreen.Image = new TextPatternIntroCustomImage { Uri ="PathToImage" };
        // Or you can also use one of our default images.
        // e.g the meter device image.
        configuration.IntroScreen.Image = new TextPatternIntroMeterDevice();
        // shipping container image.
        configuration.IntroScreen.Image = new TextPatternIntroShippingContainer();
        // general text field image.
        configuration.IntroScreen.Image = new TextPatternIntroGeneralField();
        // alphabetic text field image.
        configuration.IntroScreen.Image = new TextPatternIntroAlphabeticField();

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new ColorValue("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new ColorValue("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new ColorValue("#000000");
        configuration.IntroScreen.Explanation.Text = "To scan a single line of text, please hold your device so that the camera viewfinder clearly captures the text you want to scan. Please ensure the text is properly aligned. Once the scan is complete, the text will be automatically extracted.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        // e.g the text or the background color.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new ColorValue("#C8193C");

        // Present the view controller modally.
        var result = await ScanbotSDKMain.Rtu.TextPatternScanner.LaunchAsync(configuration);
       if (result.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Retrieve the value.
        // e.g
        Console.WriteLine($"Scanned Text: "+ result.Result.RawText);
    }
}