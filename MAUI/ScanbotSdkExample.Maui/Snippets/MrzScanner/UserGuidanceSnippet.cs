using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSDK.MAUI.Mrz;

namespace ScanbotSdkExample.Maui.Snippets.MrzScanner;

public class UserGuidanceSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new MrzScannerScreenConfiguration();

        // Configure user guidances

        // Top user guidance
        // Retrieve the instance of the top user guidance from the configuration object.
        var topUserGuidance = configuration.TopUserGuidance;

        // Show the user guidance.
        topUserGuidance.Visible = true;

        // Configure the title.
        topUserGuidance.Title.Text = "Scan your Identity Document";
        topUserGuidance.Title.Color = new ColorValue("#FFFFFF");

        // Configure the background.
        topUserGuidance.Background.FillColor = new ColorValue("#7A000000");

        // Finder overlay user guidance
        // Retrieve the instance of the finder overlay user guidance from the configuration object.
        var finderUserGuidance = configuration.FinderViewUserGuidance;

        // Show the user guidance.
        finderUserGuidance.Visible = true;

        // Configure the title.
        finderUserGuidance.Title.Text = "Scan the MRZ";
        finderUserGuidance.Title.Color = new ColorValue("#FFFFFF");

        // Configure the background.
        finderUserGuidance.Background.FillColor = new ColorValue("#7A000000");

        // Present the view controller modally.
        var scannedOutput = await ScanbotSdkMain.MrzScanner.LaunchAsync(configuration);
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