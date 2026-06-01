using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.PdfOperations;

public static class CreateImagesFromPdfSnippet
{
    public static void CreateImagesFromPdf(NSUrl pdfUrl)
    {
        // Create an instance of the PDF page extractor.
        var pageExtractor = new SBSDKPDFImageExtractor(2.0f);

        // Extract the pages from the PDF and return an array of images
        var imageRefs = pageExtractor.ExtractFromPDF(pdfUrl);
    }
}