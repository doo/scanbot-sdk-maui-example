using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Check;
using ScanbotSDK.MAUI.CheckDocumentModel;

namespace ScanbotSdkExample.Maui.Snippets.CheckScanner;

public class IntroductionSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new CheckScannerScreenConfiguration();
        
        // Show the introduction screen automatically when the screen appears.
        configuration.IntroScreen.ShowAutomatically = true;

        // Configure the background color of the screen.
        configuration.IntroScreen.BackgroundColor = new ColorValue("#FFFFFF");

        // Configure the title for the intro screen.
        configuration.IntroScreen.Title.Text = "How to scan a check";

        // Configure the image for the introduction screen.
        configuration.IntroScreen.Image = new CheckNoImage();
        
        // For a custom image...
        configuration.IntroScreen.Image = new CheckIntroCustomImage
        {
            Uri = "PathToImage"
        };
        
        // Or you can also use our default one side image.
        configuration.IntroScreen.Image = new CheckIntroCustomImage();
        
        // Or you can also use our default two sides image.
        configuration.IntroScreen.Image = new CheckIntroDefaultImage();

        // Configure the color of the handler on top.
        configuration.IntroScreen.HandlerColor = new ColorValue("#EFEFEF");

        // Configure the color of the divider.
        configuration.IntroScreen.DividerColor = new ColorValue("#EFEFEF");

        // Configure the text.
        configuration.IntroScreen.Explanation.Color = new ColorValue("#000000");
        configuration.IntroScreen.Explanation.Text =
            "To quickly and securely input your check details, please hold your device over the check, so that the camera aligns with the numbers on the front of the card.\n\n" +
            "The scanner will guide you to the optimal scanning position. Once the scan is complete, your card details will automatically be extracted and processed.\n\n" +
            "Press 'Start Scanning' to begin.";

        // Configure the done button.
        configuration.IntroScreen.DoneButton.Text = "Start Scanning";
        configuration.IntroScreen.DoneButton.Background.FillColor = new ColorValue("#C8193C");

        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.Rtu.CheckScanner.LaunchAsync(configuration);
        if (scannedOutput.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed check.
        var check = new USACheck(scannedOutput.Result.Check);
        
        // Retrieve the values.
        // e.g
        Console.WriteLine($"Account number: {check.AccountNumber.Value.Text}");
        Console.WriteLine($"Transit Number: {check.TransitNumber.Value.Text}");
        Console.WriteLine($"AuxiliaryOnUs: {check.AuxiliaryOnUs?.Value?.Text}");
    } 
}