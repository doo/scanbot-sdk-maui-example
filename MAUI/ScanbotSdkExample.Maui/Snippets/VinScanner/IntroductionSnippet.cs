using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Vin;

namespace ScanbotSdkExample.Maui.Snippets.VinScanner;

public class IntroductionSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new VinScannerScreenConfiguration();
        
        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new ColorValue("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan a VIN";

        // Configure the image for the introduction screen.
        configuration.IntroScreen.Image = new VinIntroNoImage();
        
        // For a custom image...
        configuration.IntroScreen.Image = new VinIntroCustomImage
        {
            Uri = "PathToImage"
        };
        
        // Or you can also use our default one side image.
        configuration.IntroScreen.Image = new VinIntroCustomImage();

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new ColorValue("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new ColorValue("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new ColorValue("#000000");
        configuration.IntroScreen.Explanation.Text =
            "To scan a VIN (Vehicle Identification Number), please hold your device so that the camera viewfinder clearly captures the VIN code. Please ensure the VIN is properly aligned. Once the scan is complete, the VIN will be automatically extracted.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new ColorValue("#C8193C");

        // Present the view controller modally.
        var scannedOutput = await ScanbotSdkMain.VinScanner.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Print the scanned text results
        Console.WriteLine("Scanned Vin Scanner: "+ scannedOutput.Result.TextResult?.RawText);
    } 
}