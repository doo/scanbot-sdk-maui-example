using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using Microsoft.Maui.Graphics.Platform;
using static ScanbotSDK.MAUI.ScanbotSDKMain;

namespace ScanbotSdkExample.Maui;

public static partial class Snippets
{
    static void CreateTiffFromDocument(ScannedDocument scannedDocument)
    {
        var config = new TiffGeneratorParameters();
        var fileUri = scannedDocument.CreateTiffAsync(config);
    }

    static async void CreateTiffFromImage(Uri[] imageFiles)
    {
        var config = new TiffGeneratorParameters();
        var fileUri = await CommonOperations.WriteTiffAsync(imageFiles.Select(f => new FileImageSource { File = f.LocalPath }), config);
    }
}