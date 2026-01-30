using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using ScanbotSDK.MAUI.Core.TiffGeneration;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class TiffSnippets
{
    static void CreateTiffFromDocument(IScannedDocument scannedDocument)
    {
        var parameters = new TiffGeneratorParameters();
        var outputFileUri = scannedDocument.CreateTiffAsync(parameters);
    }

    static async void CreateTiffFromImage(Uri[] imageFiles)
    {
        var parameters = new TiffGeneratorParameters();
        var outputFileUri = await ScanbotSDKMain.TiffGenerator.GenerateFromImagesAsync(images: imageFiles.Select(f => new FileImageSource { File = f.LocalPath }), parameters: parameters);
    }
}