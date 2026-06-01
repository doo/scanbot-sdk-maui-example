using Android.Graphics;

namespace ScanbotSdkExample.Droid.Snippets.PdfOperations;

public static class CreateImagesFromPdfSnippet
{
    public static void CreateImagesFromPdf(IO.Scanbot.Sdk.ScanbotSDK sdk, string pdfFilePath)
    {
        try
        {
            var extractor = sdk.CreatePdfImagesExtractor();

            var imageUris = extractor.ImageUrlsFromPdf(
                pdfFile: new Java.IO.File(pdfFilePath),
                outputDir: new Java.IO.File("path/to/output/folder"),
                prefix: "image_",
                compression: Bitmap.CompressFormat.Jpeg,
                quality: 100,
                scaling: 2.0f,
                bitmapConfig: Bitmap.Config.Argb8888,
                cancelCallback: null,
                progressCallback: null
            );

            foreach (var imageUri in imageUris)
            {
                // Handle the result
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}