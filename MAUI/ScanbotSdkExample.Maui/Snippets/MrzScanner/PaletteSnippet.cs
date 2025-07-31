using ScanbotSDK.MAUI;
using ScanbotSDK.MAUI.DocumentsModel;
using ScanbotSDK.MAUI.Mrz;

namespace ScanbotSdkExample.Maui.Snippets.MrzScanner;

public class PaletteSnippet
{
    public static async Task LaunchAsync()
    {
        // Create the default configuration object.
        var configuration = new MrzScannerScreenConfiguration();

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

        // Present the view controller modally.
        var result = await ScanbotSDKMain.Rtu.MrzScanner.LaunchAsync(configuration);
        if (result.Status != OperationResult.Ok)
        {
            // Indicates that cancel was tapped or the result was unsuccessful
            return;
        }

        // Wrap the resulted generic document to the strongly typed mrz class.
        var mrz = new MRZ(result.Result.MrzDocument);

        // Retrieve the values.
        // e.g
        Console.WriteLine($"Birth Date: {mrz.BirthDate.Value.Text}, Nationality: {mrz.Nationality.Value.Text}");
    }
}