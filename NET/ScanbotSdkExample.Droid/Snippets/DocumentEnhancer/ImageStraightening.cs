using IO.Scanbot.Sdk.Documentscanner;
using IO.Scanbot.Sdk.Geometry;
using IO.Scanbot.Sdk.Image;
using ScanbotSDK.Droid.Helpers;

namespace ScanbotSdkExample.Droid.Snippets.DocumentEnhancer;

public class ImageStraightening
{
    public static void StraightenImage(IO.Scanbot.Sdk.ScanbotSDK sdk, ImageRef imageRef)
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

            // Create the document enhancer.
            var enhancer = sdk.CreateDocumentEnhancer().GetOrThrow<IDocumentEnhancer>();

            // Straighten the image using the document enhancer.
            var result = enhancer.Straighten(image: imageRef,
                parameters: straighteningParameters,
                priorCornersNormalized: []).GetOrThrow<DocumentStraighteningResult>();

            // Handle the document straightening result
            var straightenedImage = result.StraightenedImage;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}