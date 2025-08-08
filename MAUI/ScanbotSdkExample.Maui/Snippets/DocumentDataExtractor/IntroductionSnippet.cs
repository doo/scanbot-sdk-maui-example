using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentDataExtractor;

namespace ScanbotSdkExample.Maui.Snippets.DocumentDataExtractor;

public class IntroductionSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentDataExtractorScreenConfiguration();
        
        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new ColorValue("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan a document";

        // Configure the image for the introduction screen.
        configuration.IntroScreen.Image = new DocumentDataIntroNoImage();
        
        // For a custom image...
        configuration.IntroScreen.Image = new DocumentDataIntroCustomImage
        {
            Uri = "PathToImage"
        };
        
        // Or you can also use our default image.
        configuration.IntroScreen.Image = new DocumentDataIntroDefaultImage();

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new ColorValue("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new ColorValue("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new ColorValue("#000000");
        configuration.IntroScreen.Explanation.Text = "To quickly and securely scan your document details, please hold your device over the document, so that the camera aligns with all the information on the document.\n\nThe scanner will guide you to the optimal scanning position. Once the scan is complete, your document details will automatically be extracted and processed.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new ColorValue("#C8193C");

        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.Rtu.DocumentDataExtractor.LaunchAsync(configuration);
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