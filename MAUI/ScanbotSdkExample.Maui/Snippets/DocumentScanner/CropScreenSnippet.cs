using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class CropScreenSnippet
{
    private static async Task StartScannerAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Retrieve the instance of the crop configuration from the main configuration object.
        var cropScreenConfiguration = configuration.Screens.Cropping;

        // e.g disable the rotation feature.
        cropScreenConfiguration.BottomBar.RotateButton.Visible = false;

        // e.g. configure various colors.
        configuration.Appearance.TopBarBackgroundColor = new ColorValue("#C8193C");
        cropScreenConfiguration.TopBarConfirmButton.Foreground.Color = Microsoft.Maui.Graphics.Colors.White;

        // e.g. customize a UI element's text
        configuration.Localization.CroppingTopBarCancelButtonTitle = "Cancel";

        // Launch the scanner
        var result = await ScanbotSDKMain.Document.StartScannerAsync(configuration);
        if (!result.IsSuccess)
        {
            // Indicates failure in the operation. Please access the Exception object returned in `result.Error`
            return;
        }
        
        // Handle the document.
        var scannedDocument = result.Value;
    }
}