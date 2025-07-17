using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using ScanbotSDK.MAUI.Common;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class IntroductionSnippet
{
    private static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Retrieve the instance of the introduction configuration from the main configuration object.
        var introductionConfiguration = configuration.Screens.Camera.Introduction;

        // Show the introduction screen automatically when the screen appears.
        introductionConfiguration.ShowAutomatically = true;

        // Create a new introduction item.
        var firstExampleEntry = new IntroListEntry();

        // Configure the introduction image to be shown.
        firstExampleEntry.Image = new ReceiptsIntroImage();

        // Configure the text.
        firstExampleEntry.Text = new StyledText
        {
            Visible = true,
            Text = "Some text explaining how to scan a receipt",
            Color = new ColorValue("#000000"),
            UseShadow = false
        };

        // Create a second introduction item.
        var secondExampleEntry = new IntroListEntry();

        // Configure the introduction image to be shown.
        secondExampleEntry.Image = new CheckIntroImage();

        // Configure the text.
        secondExampleEntry.Text = new StyledText
        {
            Visible = true,
            Text = "Some text explaining how to scan a check",
            Color = new ColorValue("#000000"),
            UseShadow = false
        };

        // Set the items into the configuration.
        introductionConfiguration.Items = [firstExampleEntry, secondExampleEntry];

        // Set a screen title.
        introductionConfiguration.Title = new StyledText
        {
            Visible = true,
            Text = "Introduction",
            Color = new ColorValue("#000000"),
            UseShadow = true
        };

        // Apply the introduction configuration.
        configuration.Screens.Camera.Introduction = introductionConfiguration;

        // Launch the scanner
        var response = await ScanbotSDKMain.Rtu.DocumentScanner.LaunchAsync(configuration);
        if (response.Status != OperationResult.Ok)
        {
            // Indicates that the cancel button was tapped.
            return;
        }
        
        // Handle the document.
        var scannerDocument = response.Result;
    }
}