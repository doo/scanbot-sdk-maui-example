using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Document;
using ScanbotSDK.MAUI.Core.Image;
using ScanbotSDK.MAUI.Core.ImageProcessing;
using ScanbotSDK.MAUI.Image;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class FilterAndRotateOnDocumentPage
{
    // @Tag("Filter and rotate a document")
    private static async Task FilterAndRotateOnDocumentAsync(string documentUuid)
    {
        // Retrieve the scanned document (replace Guid.Empty with a real value)
        var result = ScanbotSDKMain.Document.LoadDocument(documentUuid: documentUuid);
        if (!result.IsSuccess)
        {
            // access the returned exception with `result.Error`
           return;
        }

        // Retrieve the selected document page.
        var page = result.Value.Pages.First();

        // Apply rotation and filters on the page
        var rotation = ImageRotation.Clockwise90;
        // Create the instances of the filters you want to apply.
        var filter1 = new ScanbotBinarizationFilter { OutputMode = OutputMode.Antialiased };
        var filter2 = new BrightnessFilter { Brightness = 0.4 };

        await page.ModifyPageAsync(new ModifyPageOptions
        {
            Filters = [filter1, filter2],
            Rotation = rotation
        });
    }
    // @EndTag("Filter and rotate a document")

    // @Tag("Filter and rotate an image")
    private static ImageRef FilterAndRotateOnImage(ImageRef imageRef)
    {
        // rotate image
        var result = ScanbotSDKMain.ImageProcessor.Rotate(imageRef, ImageRotation.Clockwise90);
        if (!result.IsSuccess)
        {
            // access the returned exception with `result.Error`
            return null;
        }

        imageRef = result.Value;
        
        // apply filters
        var filterResult = ScanbotSDKMain.ImageProcessor.ApplyFilters(imageRef, [
            ParametricFilter.ScanbotBinarization(OutputMode.Antialiased),
            ParametricFilter.Brightness(0.4)
        ]);

        if (!filterResult.IsSuccess)
        {
            // access the returned exception with `result.Error`
            return null;
        }
        
        return result.Value;
    }
    // @EndTag("Filter and rotate an image")
}