using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.Document;

namespace ScanbotSdkExample.Maui.Snippets.DocumentScanner;

public static class PaletteSnippet
{
    private static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new DocumentScanningFlow();

        // Retrieve the instance of the palette from the configuration object.
        var palette = configuration.Palette;

        // Configure the colors.
        // The palette already has the default colors set, so you don't have to always set all the colors.
        palette.SbColorPrimary = new ColorValue("#C8193C");
        palette.SbColorPrimaryDisabled = new ColorValue("#F5F5F5");
        palette.SbColorNegative = new ColorValue("#FF3737");
        palette.SbColorPositive = new ColorValue("#4EFFB4");
        palette.SbColorWarning = new ColorValue("#FFCE5C");
        palette.SbColorSecondary = new ColorValue("#FFEDEE");
        palette.SbColorSecondaryDisabled = new ColorValue("#F5F5F5");
        palette.SbColorOnPrimary = new ColorValue("#FFFFFF");
        palette.SbColorOnSecondary = new ColorValue("#C8193C");
        palette.SbColorSurface = new ColorValue("#FFFFFF");
        palette.SbColorOutline = new ColorValue("#EFEFEF");
        palette.SbColorOnSurfaceVariant = new ColorValue("#707070");
        palette.SbColorOnSurface = new ColorValue("#000000");
        palette.SbColorSurfaceLow = new ColorValue("#26000000");
        palette.SbColorSurfaceHigh = new ColorValue("#7A000000");
        palette.SbColorModalOverlay = new ColorValue("#A3000000");

        // Launch the scanner
        var response = await ScanbotSDKMain.Rtu.DocumentScanner.LaunchAsync(configuration);
        if (response.Status != OperationResult.Ok)
        {
            // Indicates that the cancel button was tapped.
            return;
        }
        
        // Handle the document.
        var scannerDocument = response.Result;
    }
}