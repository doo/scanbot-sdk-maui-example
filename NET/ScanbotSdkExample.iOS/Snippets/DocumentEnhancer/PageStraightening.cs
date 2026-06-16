using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.DocumentEnhancer;

public class PageStraightening
{
    public static void StraightenPage(SBSDKScannedPage page)
    {
        try
        {
            var straighteningParameters = new SBSDKDocumentStraighteningParameters
            {
                // Configure the straightening mode as needed
                StraighteningMode = SBSDKDocumentStraighteningMode.Straighten,
                // The straightening parameters can be customized to fit the expected aspect ratio of the document to be straightened.
                // This can help the straightening algorithm to achieve better results.
                AspectRatios =
                [
                    new SBSDKAspectRatio(width: 5, height: 7),
                    new SBSDKAspectRatio(width: 1, height: 1),
                    new SBSDKAspectRatio(width: 16, height: 9),
                    new SBSDKAspectRatio(width: 3, height: 4)
                ]
            };

            // Straighten the page.
            page.ApplyWithRotation(rotation: SBSDKImageRotation.None,
                polygon: page.Polygon,
                filters: [],
                straighteningParameters: straighteningParameters,
                error: out var error).GetOrThrow(error);
        }
        catch (Exception e)
        {
            // handle the error thrown from the GetOrThrow(...) function, referenced by the PresentOn(...) error object.
            Console.WriteLine(e);
        }
    }
}