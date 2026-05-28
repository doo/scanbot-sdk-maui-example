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

            var document = sdk.DocumentApi.CreateDocument(documentImageSizeLimit: 2048).GetOrThrow<Document>();

            foreach (var imageUri in imageUris)
            {
                var bitmap = BitmapFactory.DecodeFile(imageUri.Path);

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