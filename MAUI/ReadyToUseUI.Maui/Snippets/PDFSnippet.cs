using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using Microsoft.Maui.Graphics.Platform;
using static ScanbotSDK.MAUI.ScanbotSDKMain;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
    static void CreatePdfFromDocument(ScannedDocument scannedDocument)
    {
        var config = new PDFConfiguration();

        var fileUri = scannedDocument.CreatePdfAsync(config);
    }

    // void CreatePdfFromImage(PlatformImage image)
    // {
    //     CommonOperations.CreatePdfAsync();
    // }
}