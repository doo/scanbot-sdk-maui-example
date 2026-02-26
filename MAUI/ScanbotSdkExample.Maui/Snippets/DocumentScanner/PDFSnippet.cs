using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.PdfGeneration;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class PdfSnippets
{
    // @Tag("Create PDF from Document")
    static async Task CreatePdfFromDocumentAsync(IScannedDocument scannedDocument)
    {
        var config = new PdfConfiguration();
        var result = await scannedDocument.CreatePdfAsync(config);
        if (result.IsSuccess)
        {
            // access the result PdfUri
            Uri outputUri = scannedDocument.PdfUri;
        }
    }
    // @EndTag("Create PDF from Document")

    // @Tag("Create PDF from Image File")
    static async Task CreatePdfFromImageAsync(Uri[] imageFiles)
    {
        var config = new PdfConfiguration();
        var result = await ScanbotSDKMain.PdfGenerator.GenerateFromImagesAsync(
            images: imageFiles.Select(f => new FileImageSource { File = f.LocalPath }), pdfConfiguration: config);
        if (result.IsSuccess)
        {
            // Access the result Uri
            Uri outputUri = result.Value;
        }
    }
    // @EndTag("Create PDF from Image File")
}