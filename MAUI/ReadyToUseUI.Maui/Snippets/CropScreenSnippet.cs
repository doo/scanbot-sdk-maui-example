using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ReadyToUseUI.Maui;

public static partial class Snippets
{
    private static async Task CropScreenSnippet()
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

        try
        {
            var document = await ScanbotSDKMain.Rtu.DocumentScanner.LaunchAsync(configuration);
            // Handle the document.
        }
        catch (TaskCanceledException)
        {
            // Indicates that the cancel button was tapped.
        }
    }
}