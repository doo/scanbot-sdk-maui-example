using IO.Scanbot.Sdk.Docprocessing;
using IO.Scanbot.Sdk.Documentscanner;
using IO.Scanbot.Sdk.Geometry;
using IO.Scanbot.Sdk.Image;

namespace ScanbotSdkExample.Droid.Snippets.DocumentEnhancer;

public class PageStraightening
{
    public static void StraightenPage(Page page)
    {
        try
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

            // Straighten the page.
            _ = page.Apply(newImageRotation: ImageRotation.None,
                newPolygon: page.Polygon,
                newFilters: [],
                newStraighteningParameters: straighteningParameters).OrThrow;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}