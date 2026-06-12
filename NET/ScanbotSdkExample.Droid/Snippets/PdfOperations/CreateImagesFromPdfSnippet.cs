using Android.Graphics;
using ScanbotSDK.Droid.Helpers;

namespace ScanbotSdkExample.Droid.Snippets.PdfOperations;

public static class CreateImagesFromPdfSnippet
{
    public static void CreateImagesFromPdf(IO.Scanbot.Sdk.ScanbotSDK sdk, string pdfFilePath)
    {
        try
        {
            var extractor = sdk.CreatePdfImagesExtractor();
            
            var pdfExtractorResult = extractor.Extract(
                pdfFile: new Java.IO.File(pdfFilePath),
                outputDir: new Java.IO.File("path/to/output/folder"),
                prefix: "image_",
                quality: 100,
                scaling: 2.0f);
            
            var imageUris = pdfExtractorResult.GetOrThrow<global::Java.Util.ArrayList>()?.ToArray() ?? [];
            foreach (var boxedUri in imageUris)
            {
                if (boxedUri is not Android.Net.Uri uri) continue;
                // Handle the Uri items here.
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}