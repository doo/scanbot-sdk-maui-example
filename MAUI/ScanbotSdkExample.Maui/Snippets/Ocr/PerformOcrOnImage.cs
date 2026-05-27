using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.Ocr;

namespace ScanbotSdkExample.Maui.Snippets.Ocr;

public static class OcrSnippet
{
    public static async Task PerformOcrOnImageAsync(ImageSource image)
    {
        var result =
            await ScanbotSDKMain.OcrEngine.RecognizeOnImagesAsync(images: [image],
                configuration: OcrConfiguration.ScanbotOcr);

        if (!result.IsSuccess)
        {
            return;
        }

        Console.WriteLine(result.Value.RecognizedText);
    }
}