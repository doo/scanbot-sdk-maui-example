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

    static async void CreateTiffFromImage(Uri[] imageFiles)
    {
        var config = new TiffOptions();
        var fileUri = await CommonOperations.WriteTiffAsync(imageFiles.Select(f => new FileImageSource { File = f.LocalPath }), config);
    }
}