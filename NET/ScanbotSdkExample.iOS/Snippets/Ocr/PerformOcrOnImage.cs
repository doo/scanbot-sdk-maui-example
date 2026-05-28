using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.Ocr;

public static class OcrSnippet
{
    public static void PerformOcrOnImage(SBSDKImageRef imageRef)
    {
        var ocrConfiguration = SBSDKOCREngineConfiguration.ScanbotOCR;

        var manager = new SBSDKOCREngineManager(ocrConfiguration);
        manager.RecognizeFromImage(imageRef, (ocrResult, error) =>
        {
            if (error != null)
            {
                Console.WriteLine(error.LocalizedDescription);
                return;
            }

            Console.WriteLine(ocrResult.RecognizedText);
        });
    }
}