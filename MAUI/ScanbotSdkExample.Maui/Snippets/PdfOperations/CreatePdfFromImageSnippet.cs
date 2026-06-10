using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Common;
using ScanbotSDK.MAUI.Core.Ocr;
using ScanbotSDK.MAUI.Core.PdfGeneration;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.Snippets.PdfOperations;

public class CreatePdfFromImageSnippet
{
    public static async Task CreatePdfFromImageAsync()
    {
        var image = await ImagePicker.PickImageAsSourceAsync();
        if (image is null) return;

        var result = await ScanbotSDKMain.PdfGenerator.GenerateFromImagesAsync(images: [image], 
                pdfConfiguration: new PdfConfiguration(), performOcr: true);

        if (result.IsSuccess)
        {
            // Handle the result
            Console.WriteLine(result.Value.LocalPath);
        }
    }
}