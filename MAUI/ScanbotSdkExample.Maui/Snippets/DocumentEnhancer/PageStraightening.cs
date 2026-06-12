using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Document;
using ScanbotSDK.MAUI.Core.DocumentScanner;
using ScanbotSDK.MAUI.Core.Geometry;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.Snippets.DocumentEnhancer;

public class PageStraightening
{
    private static async Task StraighteningDocument(string documentUuid)
    {
        // Retrieve the scanned document (replace Guid.Empty with a real value)
        var documentResult = ScanbotSDKMain.Document.LoadDocument(documentUuid: documentUuid);
        if (!documentResult.IsSuccess)
        {
            // access the returned exception with `result.Error`
            return;
        }

        // Retrieve the selected document page.
        var page = documentResult.Value.Pages.First();

        var straighteningParameters = new DocumentStraighteningParameters
        {
            // Configure the straightening mode as needed
            StraighteningMode = DocumentStraighteningMode.Straighten,
            // The straightening parameters can be customized to fit the expected aspect ratio of the document to be straightened.
            // This can help the straightening algorithm to achieve better results.
            AspectRatios =
            [
                new AspectRatio(width: 5, height: 7),
                new AspectRatio(width: 1, height: 1),
                new AspectRatio(width: 16, height: 9),
                new AspectRatio(width: 3, height: 4)
            ]
        };

        var result = await page.ModifyPageAsync(new ModifyPageOptions
        {
            StraighteningParameters = straighteningParameters
        });
        
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
        }
    }
}