using ScanbotSDK.iOS;

namespace ScanbotSdkExample.iOS.Snippets.PdfOperations;

public static class CreateDocumentFromPdfSnippet
{
    public static void CreateDocumentFromPdf(NSUrl pdfUrl)
    {
        // Create an instance of the PDF page extractor.
        var pageExtractor = new SBSDKPDFImageExtractor(2.0f);

        // Synchronously extract the pages from PDF and return them as SBSDKScannedDocument.
        // Each page of the PDF will be a separate SBSDKScannedPage.
        var scannedDocument = pageExtractor.ScannedDocumentFromPDF(pdfUrl, 2.0f, out _);
    }
}