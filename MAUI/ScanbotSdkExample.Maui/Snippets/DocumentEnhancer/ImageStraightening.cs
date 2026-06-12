using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.DocumentScanner;
using ScanbotSDK.MAUI.Core.Geometry;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.Snippets.DocumentEnhancer;

public class ImageStraightening
{
    private static async Task StraighteningImage()
    {
        var image = await ImagePicker.PickImageAsSourceAsync();
        if (image is null) return;

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

        var result = await ScanbotSDKMain.DocumentEnhancer.StraightenImageAsync(
            image,
            straighteningParameters
        );

        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }

        // Handle the document straightening result
        var straightenedImage = result.Value.StraightenedImage;
    }
}