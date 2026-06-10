using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.DocumentScanner;
using ScanbotSDK.MAUI.Core.Geometry;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public class DocumentEnhancer 
{
    private static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Create the parameters as required
        configuration.OutputSettings.StraighteningParameters.StraighteningMode = DocumentStraighteningMode.None;
        configuration.OutputSettings.StraighteningParameters.StraighteningMode = DocumentStraighteningMode.Straighten;

        // The straightening parameters can be customized to fit the expected aspect ratio of the document
        // to be straightened. This can help the straightening algorithm to achieve better results.
        configuration.OutputSettings.StraighteningParameters.AspectRatios =
        [
            new AspectRatio(width: 1, height: 1),
            new AspectRatio(width: 16, height: 9),
            new AspectRatio(width: 3, height: 4)
        ];

        // Launch the scanner
        var result = await ScanbotSDKMain.Document.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }

        // Handle the document.
        var scannedDocument = result.Value;
    }
}