using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using Microsoft.Maui.Graphics.Platform;
using static ScanbotSDK.MAUI.ScanbotSDKMain;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class TiffSnippets
{
    static void CreateTiffFromDocument(ScannedDocument scannedDocument)
    {
        var parameters = new TiffGeneratorParameters();
        var outputFileUri = scannedDocument.CreateTiffAsync(parameters);
    }

    static async void CreateTiffFromImage(Uri[] imageFiles)
    {
        var parameters = new TiffGeneratorParameters();
        var outputFileUri = await CommonOperations.WriteTiffAsync(sourceImages: imageFiles.Select(f => new FileImageSource { File = f.LocalPath }), sourceImagesEncrypted: false, parameters: parameters);
    }
}