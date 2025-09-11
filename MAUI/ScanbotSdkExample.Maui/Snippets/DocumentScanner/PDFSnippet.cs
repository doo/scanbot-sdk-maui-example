using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using static ScanbotSDK.MAUI.ScanbotSDKMain;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class PdfSnippets
{
    static async void CreatePdfFromDocument(ScannedDocument scannedDocument)
    {
        var config = new PdfConfiguration();
        var outputFileUri = await scannedDocument.CreatePdfAsync(config);
    }

    static async void CreatePdfFromImage(Uri[] imageFiles)
    {
        var config = new PdfConfiguration();
        var outputFileUri = await CommonOperations.CreatePdfAsync(sourceImages: imageFiles.Select(f => new FileImageSource { File = f.LocalPath }), sourceImagesEncrypted: false, configuration: config);
    }
}