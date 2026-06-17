using ScanbotSDK.MAUI.Core.Document;
using ScanbotSDK.MAUI.Core.DocumentScanner;
using ScanbotSDK.MAUI.Core.Geometry;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentEnhancer;

public class PageStraightening
{
    private static async Task StraighteningDocument(IScannedDocument.IPage page)
    {
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