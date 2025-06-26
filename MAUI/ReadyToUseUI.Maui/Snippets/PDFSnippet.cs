using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using static ScanbotSDK.MAUI.ScanbotSDKMain;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
    static async void CreatePdfFromDocument(ScannedDocument scannedDocument)
    {
        var config = new PdfConfiguration();
        var fileUri = await scannedDocument.CreatePdfAsync(config);
    }

    static async void CreatePdfFromImage(Uri[] imageFiles)
    {
        var config = new PdfConfiguration();
        var fileUri = await CommonOperations.CreatePdfAsync(imageFiles.Select(f => new FileImageSource { File = f.LocalPath }), config);
    }
}