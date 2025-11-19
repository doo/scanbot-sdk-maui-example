using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentData;

namespace ScanbotSdkExample.Maui.Snippets.DocumentDataExtractor;

public class LocalizationSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentDataExtractorScreenConfiguration();
        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.TopUserGuidance = "Localized topUserGuidance";
        localization.CameraPermissionCloseButton = "Localized cameraPermissionCloseButton";
        
        // Present the view controller modally.
        var scannedOutput = await ScanbotSdkMain.DocumentDataExtractor.LaunchAsync(configuration);
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