using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Core.DocumentScanner;
using ScanbotSDK.MAUI.Image;
using ScanbotSdkExample.Maui.Utils;

namespace ScanbotSdkExample.Maui.Snippets.ImageRefs;

public class ScanFromImageRefSnippet
{
    static async Task ScanDocumentFromImageRefAsync()
    {
        // Select an image from the Image Library
        var path = "my-image-uri-path";
        var imageRef = ImageRef.FromPath(path);

        // Scan a document from ImageRef
        var result =
            await ScanbotSDKMain.Document.ScanFromImageAsync(imageRef,
                configuration: new DocumentScannerConfiguration());

        if (!result.IsSuccess)
        {
            await Alert.ShowAsync(result.Error);
            return;
        }

        var documentScanningResult = result.Value;
    }
}