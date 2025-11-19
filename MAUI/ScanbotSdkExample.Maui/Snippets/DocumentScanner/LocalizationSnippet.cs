using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class LocalizationSnippet
{
    private static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Retrieve the instance of the localization from the configuration object.
        var localization = configuration.Localization;

        // Configure the strings.
        localization.CameraTopBarTitle = "document.camera.title";
        localization.ReviewScreenSubmitButtonTitle = "review.submit.title";
        localization.CameraUserGuidanceNoDocumentFound = "camera.userGuidance.noDocumentFound";
        localization.CameraUserGuidanceTooDark = "camera.userGuidance.tooDark";

        // Launch the scanner
        var response = await ScanbotSdkMain.DocumentScanner.StartScannerAsync(configuration);
        if (response.Status != OperationResult.Ok)
        {
            // Indicates that the cancel button was tapped.
            return;
        }
        
        // Handle the document.
        var scannedDocument = response.Result;
    }
}