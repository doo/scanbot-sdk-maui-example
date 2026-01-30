using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Image;
using ScanbotSDK.MAUI.Core.ImageProcessing;
using ScanbotSDK.MAUI.Image;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class FilterAndRotateOnDocumentPage
{
    private static async Task StartScannerAsync(Guid documentUuid)
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

        await page.ModifyPageAsync(filters: [filter1, filter2], rotation: rotation);
    }

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
}