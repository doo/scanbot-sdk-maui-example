using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;
using ScanbotSDK.MAUI.Core.TiffGeneration;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class TiffSnippets
{
    // @Tag("Create Tiff from Document")
    static async Task CreateTiffFromDocumentAsync(IScannedDocument scannedDocument)
    {
        var parameters = new TiffGeneratorParameters();
        var outputFileUri = await scannedDocument.CreateTiffAsync(parameters);
    }
    // @EndTag("Create Tiff from Document")

    // @Tag("Create Tiff from Image File")
    static async Task CreateTiffFromImageAsync(Uri[] imageFiles)
    {
        var parameters = new TiffGeneratorParameters();
        var outputFileUri = await ScanbotSDKMain.TiffGenerator.GenerateFromImagesAsync(images: imageFiles.Select(f => new FileImageSource { File = f.LocalPath }), tiffGeneratorParameters: parameters);
    }
    // @EndTag("Create Tiff from Image File")
}