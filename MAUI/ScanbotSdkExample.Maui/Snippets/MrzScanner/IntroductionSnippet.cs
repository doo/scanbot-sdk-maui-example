using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSDK.MAUI.Mrz;

namespace ScanbotSdkExample.Maui.Snippets.MrzScanner;

public class IntroductionSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new MrzScannerScreenConfiguration();

        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new ColorValue("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan an MRZ";

        // Configure the image for the introduction screen.
        // If you want to have no image...
        configuration.IntroScreen.Image = new MrzIntroNoImage();

        // For a custom image...
        configuration.IntroScreen.Image = new MrzIntroCustomImage { Uri = "PathToImage" };

        // Or you can also use our default image.
        configuration.IntroScreen.Image = new MrzIntroDefaultImage();

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new ColorValue("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new ColorValue("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new ColorValue("#000000");
        configuration.IntroScreen.Explanation.Text =
            "The Machine Readable Zone (MRZ) is a special code on your ID document (such as a passport or ID card) that contains your personal information in a machine-readable format.\n\nTo scan it, simply hold your camera over the document, so that it aligns with the MRZ section. Once scanned, the data will be automatically processed, and you will be directed to the results screen.\n\nPress 'Start Scanning' to begin.";

        // Configure the done button.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new ColorValue("#C8193C");

        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.Mrz.StartScannerAsync(configuration);
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