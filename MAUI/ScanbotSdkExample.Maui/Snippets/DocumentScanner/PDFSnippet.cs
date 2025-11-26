using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.PdfGeneration;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class PdfSnippets
{
    static async void CreatePdfFromDocument(IScannedDocument scannedDocument)
    {
        var config = new PdfConfiguration();
        var outputFileUri = await scannedDocument.CreatePdfAsync(config);
    }

    static async void CreatePdfFromImage(Uri[] imageFiles)
    {
        var config = new PdfConfiguration();
        var outputFileUri = await ScanbotSDKMain.PdfGenerator.GenerateFromImagesAsync(images: imageFiles.Select(f => new FileImageSource { File = f.LocalPath }), configuration: config);
    }
}