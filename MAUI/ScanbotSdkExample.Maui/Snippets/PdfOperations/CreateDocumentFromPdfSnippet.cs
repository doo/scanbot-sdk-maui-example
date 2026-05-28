using ScanbotSDK.MAUI;

namespace ScanbotSdkExample.Maui.Snippets.PdfOperations;

public class CreateDocumentFromPdfSnippet
{
    public async Task CreateDocumentFromPdfAsync(string pdfFilePath)
    {
        // Extract images from the PDF file and add them as document pages
        var documentResult = await ScanbotSDKMain.Document.CreateDocumentFromPdfAsync(pdfFilePath);

        if (documentResult.IsSuccess)
        {
            // Handle the document
        }
    }
}