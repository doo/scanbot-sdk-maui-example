using IO.Scanbot.Sdk.Image;
using IO.Scanbot.Sdk.Ocr;
using ScanbotSDK.Droid.Helpers;

namespace ScanbotSdkExample.Droid.Snippets.Ocr;

public static class OcrSnippet
{
    public static void PerformOcrOnImage(IOcrEngineManager recognizer, ImageRef imageRef)
    {
        try
        {
            var ocrResult = recognizer.RecognizeFromImageRefs([imageRef]).GetOrThrow<OcrResult>();

            Console.WriteLine(ocrResult.RecognizedText);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}