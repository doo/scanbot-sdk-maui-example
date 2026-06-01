using ScanbotSDK.MAUI;

namespace ScanbotSdkExample.Maui.Snippets.PdfOperations;

public static class CreateImagesFromPdfSnippet
{
    public static async Task CreateImagesFromPdfAsync(Uri pdfFilePath)
    {
        // Extract images from the PDF file and add them as document pages
        var imagesResult = await ScanbotSDKMain.PdfImageExtractor.ExtractImageFilesAsync(pdfFilePath);

        if (imagesResult.IsSuccess)
        {
            // Handle the result
        }
    }
}