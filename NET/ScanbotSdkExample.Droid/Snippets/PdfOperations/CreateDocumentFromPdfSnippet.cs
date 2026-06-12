using Android.Graphics;
using IO.Scanbot.Sdk.Docprocessing;
using ScanbotSDK.Droid.Helpers;

namespace ScanbotSdkExample.Droid.Snippets.PdfOperations;

public static class CreateDocumentFromPdfSnippet
{
    public static void CreateDocumentFromPdf(IO.Scanbot.Sdk.ScanbotSDK sdk, string pdfFilePath)
    {
        try
        {
            var extractor = sdk.CreatePdfImagesExtractor();
            var pdfExtractorResult = extractor.Extract(
                pdfFile: new Java.IO.File(pdfFilePath),
                outputDir: new Java.IO.File("path/to/output/folder"),
                prefix: "image_",
                quality: 100,
                scaling: 2.0f
            );

            var imageUris = pdfExtractorResult.GetOrThrow<global::Java.Util.ArrayList>()?.ToArray() ?? [];
            var document = sdk.DocumentApi.CreateDocument(documentImageSizeLimit: 2048).GetOrThrow<Document>();

            foreach (var boxedUri in imageUris)
            {
                if (boxedUri is not Android.Net.Uri uri) continue;
                // Handle the Uri items here.
                
                var bitmap = BitmapFactory.DecodeFile(uri.Path);
                if (bitmap == null)
                {
                    Console.WriteLine("Failed to load bitmap from URI");
                    continue;
                }

                document.AddPage(bitmap);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}