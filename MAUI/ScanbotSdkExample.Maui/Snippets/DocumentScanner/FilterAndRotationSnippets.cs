using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Image;
using ScanbotSDK.MAUI.Core.ImageProcessing;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class FilterAndRotateOnDocumentPage
{
    private static async Task LaunchAsync(Guid documentUuid)
    {
        // Retrieve the scanned document (replace Guid.Empty with a real value)
        var document = ScanbotSdkMain.DocumentScanner.LoadDocument(documentUuid: documentUuid);

        // Retrieve the selected document page.
        var page = document.Pages.First();

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
        imageRef = ScanbotSdkMain.ImageProcessor.Rotate(imageRef, ImageRotation.Clockwise90);
        
        // apply filters
        imageRef = ScanbotSdkMain.ImageProcessor.ApplyFilters(imageRef, [
            ParametricFilter.ScanbotBinarization(OutputMode.Antialiased),
            ParametricFilter.Brightness(0.4)
        ]);
        
        return imageRef;
    }
}