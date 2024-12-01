using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using Microsoft.Maui.Graphics.Platform;
using static ScanbotSDK.MAUI.ScanbotSDKMain;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
    static void CreateTiffFromDocument(ScannedDocument scannedDocument)
    {
        var config = new TiffOptions();

        var fileUri = scannedDocument.CreateTiffAsync(config);
    }

    // void CreateTiffFromImage(PlatformImage image)
    // {
    //     CommonOperations.CreatePdfAsync();
    // }
}