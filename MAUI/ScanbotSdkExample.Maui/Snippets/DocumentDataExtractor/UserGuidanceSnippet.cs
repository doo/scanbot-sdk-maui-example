using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.DocumentData;

namespace ScanbotSdkExample.Maui.Snippets.DocumentDataExtractor;

public class UserGuidanceSnippet
{
    public static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentDataExtractorScreenConfiguration();
        
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

        // Scan status user guidance
        // Retrieve the instance of the user guidance from the configuration object.
        var scanStatusUserGuidance = configuration.ScanStatusUserGuidance;
        scanStatusUserGuidance.Visibility = true;
        
        // Configure the title.
        scanStatusUserGuidance.Title.Text = "How to scan an ID document";
        scanStatusUserGuidance.Title.Color = new ColorValue("#FFFFFF");
        
        // Configure the background.
        scanStatusUserGuidance.Background.FillColor = new ColorValue("#7A000000");
        
        // Present the view controller modally.
        var scannedOutput = await ScanbotSDKMain.DocumentDataExtractor.StartScannerAsync(configuration);
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