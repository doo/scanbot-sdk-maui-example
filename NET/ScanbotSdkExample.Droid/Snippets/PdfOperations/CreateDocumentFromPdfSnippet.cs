using IO.Scanbot.Sdk.Docprocessing;
using IO.Scanbot.Sdk.Image;
using IO.Scanbot.Sdk.Pdf;
using ScanbotSDK.Droid.Helpers;

namespace ScanbotSdkExample.Droid.Snippets.PdfOperations;

public static class CreateDocumentFromPdfSnippet
{
    public static void CreateDocumentFromPdf(IO.Scanbot.Sdk.ScanbotSDK sdk, string pdfFilePath)
    {
        try
        {
            var document = sdk.DocumentApi.CreateDocument(documentImageSizeLimit: 2048).GetOrThrow<Document>();
            
            var extractor = sdk.CreatePdfImagesExtractor();
            extractor.Extract(pdfFile: new Java.IO.File(pdfFilePath), processingCallback: new PdfImageExtractingCallback(document)).GetOrThrow<Java.Lang.Void>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    private class PdfImageExtractingCallback(Document document) : Java.Lang.Object, IPdfImagesExtractor.IPdfImageExtractingCallback
    {
        public bool Process(ImageRef extractedImage)
        {
            using (extractedImage)
            {
                document.AddPage(extractedImage).GetOrThrow<Java.Lang.Void>();
            }

            return true;
        }
    }
}